using System;

namespace Tarantool.Net.IProto.Requests
{
    public class InsertReplaceRequest : RequestBase
    {
        public InsertReplaceRequest()
        {
            SpaceId = 0;
            Tuple = new Tuple();
        }

        public UInt32 SpaceId { get; set; }
        public Tuple Tuple { get; set; }
    }
}

