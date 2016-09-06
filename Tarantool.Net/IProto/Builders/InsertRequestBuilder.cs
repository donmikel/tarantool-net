using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class InsertRequestBuilder : RequestBuilder
    {
        private readonly InsertReplaceRequest _insertReplaceRequest;

        public InsertRequestBuilder(InsertReplaceRequest insertReplaceRequest) : base(insertReplaceRequest)
        {
            _insertReplaceRequest = insertReplaceRequest;
        }

        /// <summary>
        /// Set Space Id
        /// </summary>
        /// <param name="spaceId">Space Id</param>
        /// <returns></returns>
        public InsertRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _insertReplaceRequest.SpaceId = spaceId;
            return this;
        }

        public InsertRequestBuilder WithTuple(Func<TupleBuilder, TupleBuilder> insertTupleBuilder)
        {
            var tb = new TupleBuilder(_insertReplaceRequest.Tuple);
            _insertReplaceRequest.Tuple = insertTupleBuilder(tb);
            return this;
        }

        public InsertRequestBuilder WithTuple(object[] tuple)
        {
            _insertReplaceRequest.Tuple = new Tuple(tuple);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.INSERT)
                .WithInstruction(Key.SPACE, _insertReplaceRequest.SpaceId)
                .WithTuple(_insertReplaceRequest.Tuple);

            return this;
        }

        public static implicit operator InsertReplaceRequest(InsertRequestBuilder insertRequestBuilder)
        {
            return insertRequestBuilder._insertReplaceRequest;
        }
    }
}