using System;
using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Net.IProto
{
    public class RequestBody : IPackable
    {
        private readonly Dictionary<UInt32, object> _bodyParts = new Dictionary<UInt32, object>();

        [MessagePackMember(0)]
        public Dictionary<UInt32, object> Parts => _bodyParts;

        public void AddBodyData<T>(Key key, T value)
        {
            _bodyParts[(UInt32)key] = value;
        }

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackMapHeader(_bodyParts.Count);
            foreach (var part in _bodyParts)
            {
                packer.Pack(part.Key);
                packer.Pack(part.Value);
            }
        }
    }
}

