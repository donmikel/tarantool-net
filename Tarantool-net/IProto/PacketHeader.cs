namespace Tarantool.Net.IProto
{
    public class PacketHeader
    {
        public PacketHeader(Command command, int sync, int schemaId = 0)
        {
            Command = command;
            SchemaId = schemaId;
            Sync = sync;
        }

        public Command Command { get; private set; }

        public int SchemaId { get; private set; }
        public int Sync { get; private set; }

        public byte[] Raw
        {
            get
            {
                var temp = new byte[]
                {
                    0x82, (byte)Key.CODE, (byte)Command, (byte)Key.SYNC, 0xce, (byte)(Sync >> 24), (byte)(Sync >> 16),
                    (byte)(Sync >> 8), (byte)Sync
                };

                return temp;
            }
        }
    }
}

