using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using Tarantool.Net.IProto;
using Xunit;
using Xunit.Abstractions;

namespace Tarantool.Net.Tests
{
    public class TarantoolClientTest : TestKit
    {
        private readonly IActorRef _tarantool;
        public TarantoolClientTest()
        {
            _tarantool = ActorOfAsTestActorRef<Tarantool>(Tarantool.Create(host: "10.20.15.10", port: 3302, authToken: new AuthToken("podmogov", "123"), listeners: new HashSet<IActorRef> { TestActor }));

            ExpectMsg<Connection.Connecting>(m => m.Host == "10.20.15.10" && m.Port == 3302);
            ExpectMsg<Connection.Connected>(TimeSpan.FromSeconds(30));
        }

        [Fact(DisplayName = "Should respond to ping")]
        public void Ping()
        {
            _tarantool.Tell(Request.Ping().Build());
            ExpectMsg<Response>(r => !r.IsError);
        }

        [Fact]
        public void Select()
        {
            _tarantool.Tell(Request
                .Select()
                .WithSpaceId(512)
                .WithIndexId(0)
                .WithIterator(Iterator.ALL)
                .Build()
                );
            
            ExpectMsg<Response>(r => !r.IsError);
        }
    }
}