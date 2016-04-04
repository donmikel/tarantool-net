using System;
using System.Linq;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class CallRequestBuilder : RequestBuilder
    {
        private readonly CallRequest _callRequest;

        public CallRequestBuilder(CallRequest callRequest) : base(callRequest)
        {
            _callRequest = callRequest;
        }

        public CallRequestBuilder WithFunction(string name)
        {
            _callRequest.FunctionName = name;
            return this;
        }

        public CallRequestBuilder WithParams(params object[] parameters)
        {
            _callRequest.Params = parameters.ToList();
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.CALL)
                .WithInstruction(Key.FUNCTION, _callRequest.FunctionName)
                .WithParams(_callRequest.Params);

            return this;
        }

        public static implicit operator CallRequest(CallRequestBuilder callRequestBuilder)
        {
            return callRequestBuilder._callRequest;
        }
    }
}