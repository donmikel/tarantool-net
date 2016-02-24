using System;

namespace Tarantool.Net
{
    public class AuthToken
    {
        public AuthToken(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }

        public static AuthToken GuestToken => new AuthToken("guest", string.Empty);
    }
}