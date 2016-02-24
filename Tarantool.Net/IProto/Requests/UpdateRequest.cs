using System;

namespace Tarantool.Net.IProto.Requests
{
    public class UpdateRequest : RequestBase
    {
        public UpdateRequest()
        {
            SpaceId = 0;
            IndexId = 0;
            Key = new Tuple();
            UpdateOperations = new OperationsTuple();
        }

        public UInt32 SpaceId { get; set; }
        public UInt32 IndexId { get; set; }
        
        /// <summary>
        /// Key to be updated
        /// </summary>
        public Tuple Key { get; set; }
        public OperationsTuple UpdateOperations { get; set; }
    }
}

