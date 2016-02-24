using Tarantool.Net.IProto.Builders;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto
{
    public class Request
    {       
        public static AuthRequestBuilder Auth()
        {
            return new AuthRequestBuilder(new AuthRequest());
        }

        public static PingRequestBuilder Ping()
        {
            return new PingRequestBuilder(new PingRequest());
        }

        public static SelectRequestBuilder Select()
        {
            return new SelectRequestBuilder(new SelectRequest());
        }

        public static UpdateRequestBuilder Update()
        {
            return new UpdateRequestBuilder(new UpdateRequest());
        }

        public static InsertRequestBuilder Insert()
        {
            return new InsertRequestBuilder(new InsertReplaceRequest());
        }

        public static DeleteRequestBuilder Delete()
        {
            return new DeleteRequestBuilder(new DeleteRequest());
        }

        public static CallRequestBuilder Call()
        {
            return new CallRequestBuilder(new CallRequest());
        }

        public static EvalRequestBuilder Eval()
        {
            return new EvalRequestBuilder(new EvalRequest());
        }

        public static UpsertRequestBuilder Upsert()
        {
            return new UpsertRequestBuilder(new UpsertRequest());
        }
    }
}
