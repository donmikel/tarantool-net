using System;
using System.Text;

namespace Tarantool.Net.IProto
{
    public class GreatingResponse
    {
        public GreatingResponse(string version, string salt)
        {
            Version = version;
            Salt = salt;
        }

        public string Version { get; private set; }
        public string Salt { get; private set; }

        public static GreatingResponse Parse(byte[] value)
        {
            byte[] ver = new byte[63];
            Array.Copy(value, 0, ver, 0, 63);

            byte[] salt = new byte[44];
            Array.Copy(value, 64, salt, 0, 44);

            return new GreatingResponse(Encoding.ASCII.GetString(ver), Encoding.ASCII.GetString(salt));
        }

    }
}