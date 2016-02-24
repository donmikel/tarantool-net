using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util.Internal;
using Tarantool.Net.IProto;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net
{
    public class Tarantool : TarantoolConnectionSupervisor, IWithUnboundedStash
    {
        public static Props Create(string host = "localhost", int port = 3301, AuthToken authToken = null, HashSet<IActorRef> listeners = null, bool? stashMessage = null,
            TimeSpan? connectionTimeOut = null, TimeSpan? connectionRetryDelay = null,
            int? connectionRetryAttempts = null, TimeSpan? connectionHeartbeatDelay = null)
        {
            var config = ConfigurationFactory.Load();
            return Props.Create<Tarantool>(
                host,
                port,
                authToken ?? AuthToken.GuestToken,
                listeners ?? new HashSet<IActorRef>(),
                stashMessage ?? config.GetBoolean("tarantool.connection.stash", true),
                connectionTimeOut ?? config.GetTimeSpan("tarantool.connection.timeout", TimeSpan.FromSeconds(2)),
                connectionRetryDelay ?? config.GetTimeSpan("tarantool.connection.retry.delay", TimeSpan.FromSeconds(1)),
                connectionRetryAttempts ?? config.GetInt("tarantool.connection.retry.attempts", 3),
                connectionHeartbeatDelay
                );

        }

        public class AuthenticationFailed : Connection.StateChange
        {
            public AuthenticationFailed(string host, int port)
            {
                Host = host;
                Port = port;
            }

            public string Host { get; }
            public int Port { get; }
        }

        private readonly string _host;
        private readonly int _port;
        private readonly bool _stashMessage;
        private readonly HashSet<IActorRef> _listeners;
        private readonly TimeSpan _connectionRetryDelay;
        private readonly int? _connectionRetryAttempts;


        private int _retries = 0;

        public Tarantool(string host, int port, AuthToken authToken, HashSet<IActorRef> listeners, bool stashMessage, TimeSpan connectionTimeOut, TimeSpan connectionRetryDelay, int? connectionRetryAttempts, TimeSpan? connectionHeartbeatDelay) : 
            base(authToken, listeners, connectionTimeOut, connectionHeartbeatDelay)
        {
            _host = host;
            _port = port;
            _listeners = listeners;
            _connectionRetryDelay = connectionRetryDelay;
            _connectionRetryAttempts = connectionRetryAttempts;
            _stashMessage = stashMessage;

            Become(Disconnected);
        }

        //private bool NeedStash
        //{
        //    get
        //    {
                
        //    }
        //}

        protected override void PreStart()
        {
            _listeners.ForEach(l => Context.Watch(l));
            Self.Tell(new Connect(_host, _port));
        }

        protected override void Disconnected()
        {
            DisconnectedWithRetry();
            base.Disconnected();
        }

        private void DisconnectedWithRetry()
        {
            Receive<RequestBase>(_ => !_stashMessage,
                x =>
                    Sender.Tell(new Failure
                    {
                        Exception = new TarantoolDisconnectedException($"Disconnected from {_host}:{_port}")
                    }));

            Receive<RequestBase>(_ => _stashMessage, _ => Stash.Stash());

            Receive<AuthOk>(x =>
            {
                _retries = 0;
                NotifyStateChanged(x.Connected);
                if(_stashMessage)
                    Stash.UnstashAll();

                Become(Connected);

            });

            Receive<Reconnect>(_ =>
            {
                if (_connectionRetryAttempts.HasValue & _connectionRetryAttempts > _retries)
                {
                    _retries++;
                    Context.System.Scheduler.ScheduleTellOnce(_connectionRetryDelay, Connection, new Connection.Connect(), Self);
                }
                else if (!_connectionRetryAttempts.HasValue)
                {
                    Context.System.Scheduler.ScheduleTellOnce(_connectionRetryDelay, Connection, new Connection.Connect(), Self);
                }
            });
        }

    //    private void HandleListenedMessages()
    //    {
    ////          case Connection.Connecting(_, _)       ⇒ // do nothing
    ////case Connection.ConnectionFailed(_, _) ⇒ // do nothing
    ////case Sentinel.ConnectionFailed(_)      ⇒ // do nothing
    ////case Redis.AuthenticationFailed(_, _)  ⇒ //; do nothing
    ////case Connection.Connected(_, _)        ⇒ setConnected()
    ////case Connection.Disconnected(_, _)     ⇒ setDisconnected()

    //        Receive<Connection.Connecting>(_ => { });
    //        Receive<Connection.ConnectionFailed>(_ => { });
    //        Receive<AuthenticationFailed>(_ => { });
    //        Receive<Connection.Connected>(_ => { });
    //        Receive<Connection.Disconnected>(_ => SetDisconnected());
    //    }

        //private void SetDisconnected()
        //{
        //    throw new NotImplementedException();
        //}

        public IStash Stash { get; set; }
    }
}