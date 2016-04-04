using System.Collections.Generic;

namespace Tarantool.Net.IProto.Requests
{
    public class CallRequest : RequestBase
    {
        public CallRequest()
        {
            Params = new List<object>();
            FunctionName = "<Unknown>";
        }
        
        public string FunctionName { get; set; }

        public List<object> Params { get; set; }
    }
}

