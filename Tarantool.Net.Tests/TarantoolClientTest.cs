using System;
using System.Collections.Generic;
using System.Threading;
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
            _tarantool = ActorOfAsTestActorRef<Tarantool>(Tarantool.Create(port: 3301, listeners: new HashSet<IActorRef> { TestActor }));

            ExpectMsg<Connection.Connecting>();
            ExpectMsg<Connection.Connected>();
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
                .WithSpaceId(513)
                .WithIndexId(0)
                .WithIterator(Iterator.ALL)
                .Build()
                );
            
            ExpectMsg<Response>(r => !r.IsError);
        }

        [Fact]
        public void Pipeline()
        {
            _tarantool.Tell(Request.Ping().Build());
            _tarantool.Tell(Request.Ping().Build());
            _tarantool.Tell(Request.Ping().Build());

            ExpectMsg<Response>();
            ExpectMsg<Response>();
            ExpectMsg<Response>();
        }
    }
}