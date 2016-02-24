using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class EvalRequestBuilder : RequestBuilder
    {
        private readonly EvalRequest _evalRequest;

        public EvalRequestBuilder(EvalRequest evalRequest) : base(evalRequest)
        {
            _evalRequest = evalRequest;
        }

        public EvalRequestBuilder WithExpression(string expression)
        {
            _evalRequest.Expression = expression;
            return this;
        }

        public EvalRequestBuilder WithTuple(Func<TupleBuilder, TupleBuilder> evalTupleBuilder)
        {
            var etb = new TupleBuilder(_evalRequest.Tuple);
            _evalRequest.Tuple = evalTupleBuilder(etb);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.EVAL)
                .WithInstruction(Key.EXPRESSION, _evalRequest.Expression)
                .WithTuple(_evalRequest.Tuple);

            return this;
        }

        public static implicit operator EvalRequest(EvalRequestBuilder evalRequestBuilder)
        {
            return evalRequestBuilder._evalRequest;
        }
    }
}