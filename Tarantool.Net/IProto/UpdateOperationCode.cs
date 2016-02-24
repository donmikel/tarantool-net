namespace Tarantool.Net.IProto
{
    public static class UpdateOperationCode 
    {
        public const string Addition = "+";
        public const string Subtraction = "-";
        public const string BitwiseAnd = "&";
        // ReSharper disable once InconsistentNaming
        public const string BitwiseXOR = "^";
        public const string BitwiseOr  = "|";
        public const string Delete = "#";
        public const string Insert = "!";
        public const string Assign = "=";
        public const string Splice = ":";
    }
}