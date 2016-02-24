// ReSharper disable InconsistentNaming
namespace Tarantool.Net.IProto
{
    public enum Iterator
    {
        EQ = 0,
        REQ = 1,
        ALL = 2,
        LT = 3,
        LE = 4,
        GE = 5,
        GT = 6,
        BITSET_ALL_SET = 7,
        BITSET_ANY_SET = 8,
        BITSET_ALL_NOT_SET = 9
    }
}