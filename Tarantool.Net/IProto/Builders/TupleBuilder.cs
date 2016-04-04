using System;

namespace Tarantool.Net.IProto.Builders
{
    public class TupleBuilder
    {
        private readonly Tuple _tuple;

        public TupleBuilder(Tuple tuple)
        {
            _tuple = tuple;
        }

        public TupleBuilder AddField(object data)
        {
            _tuple.AddField(data);
            return this;
        }


        public static implicit operator Tuple(TupleBuilder tupleBuilder)
        {
            return tupleBuilder._tuple;
        }
    }
}