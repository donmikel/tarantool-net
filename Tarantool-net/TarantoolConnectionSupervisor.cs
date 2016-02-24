using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using Tarantool.Net.IProto;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net
{
    public abstract class TarantoolConnectionSupervisor : ReceiveActor
    {
        protected class Connect
        {
            public Connect(string host, int port)
            {
                Host = host;
                Port = port;
            }

            public string Host { get; private set; }
            public int Port { get; private set; } 
        }
        protected class Reconnect {}
        protected class AuthOk
        {
            public AuthOk(Connection.Connected connected)
            {
                Connected = connected;
            }

            public Connection.Connected Connected { get; }
        }

        protected IActorRef Connection = Context.System.DeadLetters;

        private readonly AuthToken _authToken;
        private readonly HashSet<IActorRef> _listeners;
        private readonly TimeSpan _connectionTimeOut;
        private readonly TimeSpan? _connectionHeartbeatDelay;

        protected TarantoolConnectionSupervisor(AuthToken authToken, HashSet<IActorRef> listeners, TimeSpan connectionTimeOut, TimeSpan? connectionHeartbeatDelay = null)
        {
            _authToken = authToken;
            _listeners = listeners;
            _connectionTimeOut = connectionTimeOut;
            _connectionHeartbeatDelay = connectionHeartbeatDelay;

            Status = new Connection.Disconnected("unknown", 0);
        }

        protected Connection.StateChange Status { get; private set; }

        protected void Connected()
        {
            HandleListeners();

            Receive<RequestBase>(m => Connection.Forward(m));

            Receive<Connection.Disconnected>(dm =>
            {
                NotifyStateChanged(dm);
                Become(Disconnected);
                Self.Tell(new Reconnect());
            });
        }

        protected virtual void Disconnected()
        {
            HandleListeners();

            Receive<Connect>(x =>
            {
                Connection.Tell(PoisonPill.Instance);
                Connection =
                    Context.ActorOf(Props.Create<Connection>(Self, x.Host, x.Port, _connectionTimeOut, _connectionHeartbeatDelay));
            });

            Receive<Connection.Connecting>(x => NotifyStateChanged(x));

            Receive<Connection.Connected>(x => Authenticate(x));

            Receive<AuthOk>(x =>
            {
                NotifyStateChanged(x.Connected);
                Become(Connected);
            });

            Receive<Connection.ConnectionFailed>(x =>
            {
                NotifyStateChanged(x);
                Self.Tell(new Reconnect());
            });

        }

        private void Authenticate(Connection.Connected x)
        {
            var self = Self;
            Connection.Ask(Request
                .Auth()
                .Password(_authToken.Password)
                .UserName(_authToken.Username)
                .Salt(x.Salt)
                .Build()).ContinueWith(tr =>
                {
                    if (tr.IsFaulted)
                        NotifyStateChanged(new Tarantool.AuthenticationFailed(x.Host, x.Port));

                    self.Tell(new AuthOk(x));
                });
        }

        private void HandleListeners()
        {
            Receive<IActorRef>(ar =>
            {
                _listeners.Add(ar);
                ar.Tell(Status);
            });

            Receive<Terminated>(m => _listeners.Remove(m.ActorRef));
        }

        protected void NotifyStateChanged(Connection.StateChange newState)
        {
            Status = newState;
            _listeners.ForEach(l => ActorRefImplicitSenderExtensions.Tell(l, newState));
        }
    }
}