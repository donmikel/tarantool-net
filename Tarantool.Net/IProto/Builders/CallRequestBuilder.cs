using System;
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

        public CallRequestBuilder WithTuple(Func<TupleBuilder, TupleBuilder> callTupleBuilder)
        {
            var ctb = new TupleBuilder(_callRequest.Tuple);
            _callRequest.Tuple = callTupleBuilder(ctb);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.CALL)
                .WithInstruction(Key.FUNCTION, _callRequest.FunctionName)
                .WithTuple(_callRequest.Tuple);

            return this;
        }

        public static implicit operator CallRequest(CallRequestBuilder callRequestBuilder)
        {
            return callRequestBuilder._callRequest;
        }
    }
}