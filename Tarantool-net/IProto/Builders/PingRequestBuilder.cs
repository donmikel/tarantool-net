using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class PingRequestBuilder : RequestBuilder
    {
        private readonly PingRequest _pingRequest;

        public PingRequestBuilder(PingRequest pingRequest) : base(pingRequest)
        {
            _pingRequest = pingRequest;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.PING);
            return this;
        }

        public static implicit operator PingRequest(PingRequestBuilder pingRequestBuilder)
        {
            return pingRequestBuilder._pingRequest;
        }
    }
}