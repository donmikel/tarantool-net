// ReSharper disable InconsistentNaming
namespace Tarantool.Net.IProto
{
    public enum Command
    {
        SELECT = 0x01,
        INSERT = 0x02,
        REPLACE = 0x03,
        UPDATE = 0x04,
        DELETE = 0x05,
        CALL = 0x06,
        AUTH = 0x07,
        EVAL = 0x08,
        UPSERT = 0x09,
        // Admin command codes
        PING = 0x40
    }
}