using System;

namespace Tarantool.Net.IProto.Requests
{
    public class UpsertRequest : RequestBase
    {
        public UpsertRequest()
        {
            SpaceId = 0;
            Tuple = new Tuple();
            UpsertOperations = new OperationsTuple();
        }

        public UInt32 SpaceId { get; set; }
        public Tuple Tuple { get; set; }
        public OperationsTuple UpsertOperations { get; set; }
    }
}