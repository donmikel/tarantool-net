using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class ReplaceRequestBuilder : RequestBuilder
    {
        private readonly InsertReplaceRequest _insertReplaceRequest;

        public ReplaceRequestBuilder(InsertReplaceRequest insertReplaceRequest) : base(insertReplaceRequest)
        {
            _insertReplaceRequest = insertReplaceRequest;
        }

        /// <summary>
        /// Set Space Id
        /// </summary>
        /// <param name="spaceId">Space Id</param>
        /// <returns></returns>
        public ReplaceRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _insertReplaceRequest.SpaceId = spaceId;
            return this;
        }

        public ReplaceRequestBuilder WithTuple(Func<TupleBuilder, TupleBuilder> insertTupleBuilder)
        {
            var tb = new TupleBuilder(_insertReplaceRequest.Tuple);
            _insertReplaceRequest.Tuple = insertTupleBuilder(tb);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.REPLACE)
                .WithInstruction(Key.SPACE, _insertReplaceRequest.SpaceId)
                .WithTuple(_insertReplaceRequest.Tuple);

            return this;
        }

        public static implicit operator InsertReplaceRequest(ReplaceRequestBuilder replaceRequestBuilder)
        {
            return replaceRequestBuilder._insertReplaceRequest;
        }
    }
}