using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class SelectRequestBuilder : RequestBuilder
    {
        private readonly SelectRequest _selectRequest;

        public SelectRequestBuilder(SelectRequest selectRequest) : base(selectRequest)
        {
            _selectRequest = selectRequest;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.SELECT)
                .WithInstruction(Key.SPACE, _selectRequest.SpaceId)
                .WithInstruction(Key.INDEX, _selectRequest.IndexId)
                .WithInstruction(Key.LIMIT, _selectRequest.Limit)
                .WithInstruction(Key.OFFSET, _selectRequest.Offset)
                .WithInstruction(Key.ITERATOR, (int) _selectRequest.Iterator)
                .WithInstruction(Key.KEY, _selectRequest.Key);

            return this;
        }

        public SelectRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _selectRequest.SpaceId = spaceId;
            return this;
        }

        public SelectRequestBuilder WithIndexId(UInt32 indexId)
        {
            _selectRequest.IndexId = indexId;
            return this;
        }

        public SelectRequestBuilder WithOffset(UInt32 offset)
        {
            _selectRequest.Offset = offset;
            return this;
        }

        public SelectRequestBuilder WithLimit(UInt32 limit)
        {
            _selectRequest.Limit = limit;
            return this;
        }
        public SelectRequestBuilder WithIterator(Iterator iterator)
        {
            _selectRequest.Iterator = iterator;
            return this;
        }
        public SelectRequestBuilder WithKey(Func<TupleBuilder, TupleBuilder> tupleBuilder)
        {
            var tb = new TupleBuilder(_selectRequest.Key);
            _selectRequest.Key = tupleBuilder(tb);
            return this;
        }

        public static implicit operator SelectRequest(SelectRequestBuilder selectRequestBuilder)
        {
            return selectRequestBuilder._selectRequest;
        }
    }
}