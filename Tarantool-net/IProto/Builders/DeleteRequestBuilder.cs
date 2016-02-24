using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class DeleteRequestBuilder : RequestBuilder
    {
        private readonly DeleteRequest _deleteRequest;

        public DeleteRequestBuilder(DeleteRequest deleteRequest) : base(deleteRequest)
        {
            _deleteRequest = deleteRequest;
        }

        public DeleteRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _deleteRequest.SpaceId = spaceId;
            return this;
        }

        public DeleteRequestBuilder WithIndexId(UInt32 indexId)
        {
            _deleteRequest.IndexId = indexId;
            return this;
        }

        public DeleteRequestBuilder WithKey(Func<TupleBuilder, TupleBuilder> tupleBuilder)
        {
            var tb = new TupleBuilder(_deleteRequest.Key);
            _deleteRequest.Key = tupleBuilder(tb);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.DELETE)
                .WithInstruction(Key.SPACE, _deleteRequest.SpaceId)
                .WithInstruction(Key.INDEX, _deleteRequest.IndexId)
                .WithInstruction(Key.KEY, _deleteRequest.Key);

            return this;
        }

        public static implicit operator DeleteRequest(DeleteRequestBuilder deleteRequestBuilder)
        {
            return deleteRequestBuilder._deleteRequest;
        }
    }
}