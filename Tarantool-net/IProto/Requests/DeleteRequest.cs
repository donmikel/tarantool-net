using System;

namespace Tarantool.Net.IProto.Requests
{
    public class DeleteRequest : RequestBase
    {
        public DeleteRequest()
        {
            SpaceId = 0;
            IndexId = 0;
            Key = new Tuple();
        }
        
        public UInt32 SpaceId { get; set; }
        public UInt32 IndexId { get; set; }
        public Tuple Key { get; set; }
    }
}

