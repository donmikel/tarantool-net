namespace Tarantool.Net.IProto.Requests
{
    public class EvalRequest : RequestBase
    {
        public EvalRequest()
        {
            Expression = string.Empty;
            Tuple = new Tuple();
        }

        public string Expression { get; set; }
        public Tuple Tuple { get; set; }
    }
}