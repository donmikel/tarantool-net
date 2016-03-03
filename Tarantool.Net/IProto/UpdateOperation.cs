using System;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Net.IProto
{
    public class UpdateOperation<T> : IPackable
    {
        public UpdateOperation()
        {
            FieldNo = 0;
            Code = UpdateOperationCode.Assign;
            Arg = default(T);
        }

        [MessagePackMember(1)]
        public UInt32 FieldNo { protected get; set; }

        [MessagePackMember(0)]
        public string Code { protected get; set; }

        [MessagePackMember(2)]
        public T Arg { protected get; set; }

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackArrayHeader(3);
            packer.PackString(Code);
            packer.Pack(FieldNo);
            packer.Pack(Arg);
        }
    }

    public class SpliceUpdateOperation : UpdateOperation<string>, IPackable
    {
        public SpliceUpdateOperation()
        {
            Code = UpdateOperationCode.Splice;
            Position = 0;
            Offset = 0;
        }

        public UInt32 Position { get; set; }
        public UInt32 Offset { get; set; }

        public new void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackArrayHeader(5);
            packer.PackString(Code);
            packer.Pack(FieldNo);
            packer.Pack(Position);
            packer.Pack(Offset);
            packer.PackString(Arg);
        }
    }
}