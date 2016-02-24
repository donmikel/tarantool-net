using System;
using System.Security.Cryptography;
using System.Text;

namespace Tarantool.Net.IProto.Requests
{
    public class AuthRequest : RequestBase
    {
        public AuthRequest()
        {
            Username = "guest";
            Password = string.Empty;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        private string _salt;

        public void SetSalt(string salt)
        {
            _salt = salt;
        }
        public Tuple GetAuthTuple()
        {
            var tuple = new Tuple();
            if (string.IsNullOrWhiteSpace(Password))
                return tuple;

            var salt = Convert.FromBase64String(_salt);
            var sha = new SHA1Managed();
            var step1 = sha.ComputeHash(Encoding.UTF8.GetBytes(Password));
            var step2 = sha.ComputeHash(step1);
            var scrambleSize = 20;

            var saltedHash = new byte[step2.Length + salt.Length];
            salt.CopyTo(saltedHash, 0);
            step2.CopyTo(saltedHash, salt.Length);
            sha.Clear();
            sha = new SHA1Managed();

            sha.TransformBlock(salt, 0, scrambleSize, saltedHash, 0);
            sha.TransformFinalBlock(step2, 0, step2.Length);

            var step3 = sha.Hash;
            sha.Clear();

            var xor = XOR(step1, step3);
            tuple.AddField("chap-sha1");
            tuple.AddField(xor);
            return tuple;
        }

        private byte[] XOR(byte[] buffer1, byte[] buffer2)
        {
            byte[] result = new byte[20];
            for (int i = 0; i < buffer1.Length; i++)
                result[i] = (byte)(buffer1[i] ^ buffer2[i]);
            return result;
        }
    }
}