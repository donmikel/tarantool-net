using System;

namespace Tarantool.Net.IProto.Requests
{
    public class SelectRequest : RequestBase
    {
        public SelectRequest()
        {
            SpaceId = 0;
            IndexId = 0;
            Offset = 0;
            Limit = int.MaxValue;
            Iterator = Iterator.EQ;
            Key = new Tuple();
        }

        public UInt32 SpaceId { get; set; }

        public UInt32 IndexId { get; set; }

        public UInt32 Offset { get; set; }

        public UInt32 Limit { get; set; }
        public Iterator Iterator { get; set; }
        public Tuple Key { get; set; }
    }
    
}