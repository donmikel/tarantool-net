using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.IO;
using MsgPack.Serialization;
using Tarantool.Net.IProto;
using Tarantool.Net.IProto.Requests;
using Dns = System.Net.Dns;
using Tuple = System.Tuple;

namespace Tarantool.Net
{
    public class Connection : ReceiveActor
    {
        private static readonly MessagePackSerializer<Response> Serializer =
            SerializationContext.Default.GetSerializer<Response>();
        public class StateChange { }
        public class Connecting : StateChange
        {
            public Connecting(string host, int port)
            {
                Host = host;
                Port = port;
            }

            public string Host { get; }
            public int Port { get; }
        }
        public class Connected : StateChange
        {
            public Connected(string host, int port, string salt, string version)
            {
                Port = port;
                Salt = salt;
                Version = version;
                Host = host;
            }

            public string Version { get; }
            public string Salt { get; }
            public string Host { get; }
            public int Port { get; }
        }
        public class Disconnected : StateChange
        {
            public Disconnected(string host, int port)
            {
                Port = port;
                Host = host;
            }

            public string Host { get; }
            public int Port { get; }
        }
        public class ConnectionFailed : StateChange
        {
            public ConnectionFailed(string host, int port)
            {
                Port = port;
                Host = host;
            }

            public string Host { get; }
            public int Port { get; }
        }
        private class CommandAck : Tcp.Event
        {
            public CommandAck(IActorRef sender, int sync)
            {
                Sender = sender;
                Sync = sync;
            }

            public IActorRef Sender { get; private set; }
            public int Sync { get; private set; }
        }
        private class Heartbeat
        {
            public Heartbeat(TimeSpan delay)
            {
                Delay = delay;
            }

            public TimeSpan Delay { get; private set; }
        }
        internal class Connect
        {
        }
        public class CurrentConnection
        {
            public CurrentConnection(IActorRef socket, int sync = 10)
            {
                Sync = sync;
                Socket = socket;
            }

            public int GetNextSync()
            {
                if (Sync > 1000000) Sync = 0;
                return Sync++;
            }

            public int Sync { get; private set; }
            public IActorRef Socket { get; private set; }
        }

        private readonly IActorRef _listener;
        private readonly string _host;
        private readonly int _port;
        private readonly TimeSpan _connectionTimeout;
        private readonly TimeSpan _heartbeatDelay;
        //private readonly Queue<IActorRef> _requesterQueue;
        private readonly Dictionary<int, IActorRef> _requesterDictionary;
        private GreatingResponse _greatingResponse;
        private CurrentConnection _currentConnection;
        private DateTime _lastDataReceived = DateTime.Now;
        private byte[] _responseBuffer;
        public Connection(IActorRef listener, string host, int port, TimeSpan connectionTimeout, TimeSpan? heartbeatDelay = null)
        {
            _listener = listener;
            _host = host;
            _port = port;
            _connectionTimeout = connectionTimeout;
            _heartbeatDelay = heartbeatDelay ?? TimeSpan.FromSeconds(2);
            //_requesterQueue = new Queue<IActorRef>();
            _requesterDictionary = new Dictionary<int, IActorRef>();
            _responseBuffer = new byte[] { };
            Become(EstablishConnect);
        }

        protected override void PreStart()
        {
            Self.Tell(new Connect());
            base.PreStart();
        }

        private void EstablishConnect()
        {
            Receive<Connect>(_ =>
            {
                IPAddress ip;
                if (!IPAddress.TryParse(_host, out ip))
                    ip = Dns.GetHostAddresses(_host).First();

                var endPoint = new IPEndPoint(ip, _port);
                _listener.Tell(new Connecting(_host.ToString(), _port));
                Context.System.Tcp().Tell(new Tcp.Connect(endPoint, timeout: _connectionTimeout));
            });

            Receive<Tcp.Connected>(c =>
            {
                _currentConnection = new CurrentConnection(Sender);
                _currentConnection.Socket.Tell(new Tcp.Register(Self, useResumeWriting: false));
                var greatingMessageTimer = Context.System.Scheduler.ScheduleTellOnceCancelable(_connectionTimeout, Self,
                    ReceiveTimeout.Instance, Self);

                Become(() => WaitingGreating(greatingMessageTimer));
            });

        }


        private void WaitingGreating(ICancelable greatingMessageTimer)
        {
            Receive<Tcp.Received>(data =>
            {
                _lastDataReceived = DateTime.Now;
                greatingMessageTimer.Cancel();
                _greatingResponse = GreatingResponse.Parse(data.Data.ToArray());
                var scheduler = Context.System.Scheduler;
                var self = Self;
                Self.Ask<Response>(Request.Ping().Build(), _heartbeatDelay).ContinueWith(t =>
                {
                    StateChange state = new Connected(_host.ToString(), _port, _greatingResponse.Salt, _greatingResponse.Version);
                    if (t.IsFaulted)
                        state = new ConnectionFailed(_host.ToString(), _port);
                    else
                    {
                        //Console.WriteLine($"Receive1");

                        scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), self, new Heartbeat(_heartbeatDelay), self);
                    }


                    return state;
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(_listener);
                Become(WaitingAuth);
            });

            Receive<ReceiveTimeout>(t =>
            {
                _currentConnection.Socket.Tell(Tcp.Close.Instance);
            });

            ConnectionManagement();

        }

        private void WaitingAuth()
        {
            Receive<Tcp.Received>(data =>
            {
                ReceiveData(data);
                Become(ConnectedState);
            });

            ConnectionManagement();
        }

        private void ReceiveData(Tcp.Received data)
        {
            _lastDataReceived = DateTime.Now;
            var responses = ParseResponse(data.Data.ToArray());

            foreach (var response in responses)
            {
                if (!_requesterDictionary.ContainsKey(response.Sync))
                    return;

                var sender = _requesterDictionary[response.Sync]; //_requesterQueue.Dequeue();

                if (sender != null)
                {
                    if (response.IsError)
                        sender.Tell(new Failure { Exception = new Exception(response.Error) }, Self);
                    else
                        sender.Tell(response);
                }
            }
        }

        private void ConnectedState()
        {
            Receive<Tcp.Received>(data =>
            {
                ReceiveData(data);
            });

            ConnectionManagement();

        }

        private void ConnectionManagement()
        {
            Receive<Tcp.ConnectionClosed>(_ =>
            {
                _requesterDictionary.Clear();
                _listener.Tell(new Disconnected(_host.ToString(), _port));
            });

            Receive<Tcp.CommandFailed>(p => p.Cmd is Tcp.Write, c => _currentConnection.Socket.Tell(c.Cmd));
            Receive<Tcp.CommandFailed>(p => p.Cmd is Tcp.Connect,
                c => _listener.Tell(new ConnectionFailed(_host.ToString(), _port)));

            Receive<CommandAck>(s =>
            {
                _requesterDictionary.Add(s.Sync, s.Sender);
            });

            Receive<Heartbeat>(h =>
            {
                var idle = DateTime.Now - _lastDataReceived;
                var result = Tuple.Create(idle > (h.Delay + h.Delay), idle > h.Delay);
                if (result.Item1 && result.Item2)
                    _currentConnection.Socket.Tell(Tcp.Close.Instance);
                else if (!result.Item1 && result.Item2)
                    Self.Tell(Request.Ping().Build());
            });

            ReceiveRequests();
        }

        private void ReceiveRequests()
        {
            Receive<RequestBase>(s =>
            {
                _sender = Sender;
                SendRequest(s);
            });
        }

        private IActorRef _sender = null;
        private void SendRequest(RequestBase s)
        {
            var sync = _currentConnection.GetNextSync();
            var message = s.WithSync(sync).GetBytes();
            _currentConnection.Socket.Tell(Tcp.Write.Create(ByteString.Create(message), new CommandAck(Sender, sync)));
        }

        private List<Response> ParseResponse(byte[] response)
        {
            var result = new List<Response>();
            int totalLen = response.Length;

            if (_responseBuffer.Length > 0)
            {
                totalLen = totalLen + _responseBuffer.Length;
                var temp = new byte[totalLen];
                Array.Copy(_responseBuffer, 0, temp, 0, _responseBuffer.Length);
                Array.Copy(response, 0, temp, _responseBuffer.Length, response.Length);
                response = temp;
                _responseBuffer = new byte[0];
            }

            int position = 0;
            while (position < totalLen)
            {
                position++;
                if (totalLen - position < 4)
                {
                    position = position - 1;
                    _responseBuffer = new byte[totalLen - position];
                    Array.Copy(response, position, _responseBuffer, 0, totalLen - position);
                    break;
                }
                
                var respLenArray = new byte[4];
                Array.Copy(response, position, respLenArray, 0, 4);
                Array.Reverse(respLenArray);
                var respLen = BitConverter.ToInt32(respLenArray, 0);
                position = position + 4;

                if (totalLen - position < respLen)
                {
                    position = position - 5;
                    _responseBuffer = new byte[totalLen - position];
                    Array.Copy(response, position, _responseBuffer, 0, totalLen - position);
                    break;
                }

                var resp = new byte[respLen];
                Array.Copy(response, position, resp, 0, respLen);
                position = position + respLen;

                result.Add(Serializer.UnpackSingleObject(resp));
            }

            return result;
        }

    }
}