using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class AuthRequestBuilder : RequestBuilder
    {
        private readonly AuthRequest _authRequest;

        public AuthRequestBuilder(AuthRequest authRequest) : base(authRequest)
        {
            _authRequest = authRequest;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.AUTH)
                .WithInstruction(Key.USER_NAME, _authRequest.Username)
                .WithTuple(_authRequest.GetAuthTuple());
            return this;
        }

        public AuthRequestBuilder UserName(string userName)
        {
            _authRequest.Username = userName;
            return this;
        }

        public AuthRequestBuilder Password(string password)
        {
            _authRequest.Password = password;
            return this;
        }

        public AuthRequestBuilder Salt(string salt)
        {
            _authRequest.SetSalt(salt);
            return this;
        }

        public static implicit operator AuthRequest(AuthRequestBuilder authRequestBuilder)
        {
            return authRequestBuilder._authRequest;
        }
    }
}