// ReSharper disable InconsistentNaming
namespace Tarantool.Net.IProto
{
    public enum Key 
    {
        CODE = 0x00,
        SYNC = 0x01,
        
        SCHEMA_ID = 0x05,

        SPACE = 0x10,
        INDEX = 0x11,
        LIMIT = 0x12,
        OFFSET = 0x13,
        ITERATOR = 0x14,

        KEY = 0x20,
        TUPLE = 0x21,
        FUNCTION = 0x22,
        USER_NAME = 0x23,
        EXPRESSION = 0x27,
        UPSERT_OPS = 0x28,
        DATA = 0x30,
        ERROR = 0x31
    }
}