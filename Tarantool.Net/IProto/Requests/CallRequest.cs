namespace Tarantool.Net.IProto.Requests
{
    public class CallRequest : RequestBase
    {
        public CallRequest()
        {
            Tuple = new Tuple();
            FunctionName = "<Unknown>";
        }
        
        public string FunctionName { get; set; }

        public Tuple Tuple { get; set; }
    }
}

