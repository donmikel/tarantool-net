using System;
using System.Collections.Generic;
using System.Linq;
using MsgPack;

namespace Tarantool.Net.IProto
{
    public class Response : IUnpackable
    {
        public int Code { get; set; }
        public int Sync { get; set; }
        public int SchemaId { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
        public List<Tuple> Body { get; set; }

        public void UnpackFromMessage(Unpacker unpacker)
        {
            int codeResult;
            MessagePackObject obj;
            unpacker.ReadInt32(out codeResult);
            unpacker.ReadObject(out obj);
            ParseIProtoResponse(codeResult, obj);

            unpacker.ReadInt32(out codeResult);
            unpacker.ReadObject(out obj);
            ParseIProtoResponse(codeResult, obj);

            unpacker.ReadInt32(out codeResult);
            unpacker.ReadObject(out obj);
            ParseIProtoResponse(codeResult, obj);

            unpacker.ReadObject(out obj);
            if (obj.IsDictionary)
            {
                var dict = obj.AsDictionary();
                if (dict.Keys.Count > 0)
                {
                    foreach (var key in dict.Keys)
                    {
                        var code = key.AsInt32();

                        ParseIProtoResponse(code, dict[key]);
                    }
                }
            }
        }

        private void ParseIProtoResponse(int code, MessagePackObject value)
        {
            switch (code)
            {
                case (int)Key.CODE:
                    Code = value.AsInt32();
                    break;
                case (int)Key.SYNC:
                    Sync = value.AsInt32();
                    break;
                case (int)Key.SCHEMA_ID:
                    SchemaId = value.AsInt32();
                    break;
                case (int)Key.DATA:
                    Body = value.AsList().Select(i => new Tuple(i.AsList().Select(a => a.ToObject()).ToList())).ToList();
                    break;
                case (int)Key.ERROR:
                    Error = value.AsString();
                    IsError = true;
                    break;
            }
        }
    }

    public class TarantoolException : Exception
    {
        public TarantoolException(string message) : base(message)
        {
        }
    }

    public class TarantoolDisconnectedException : Exception
    {
        public TarantoolDisconnectedException(string message) : base(message)
        {
        }
    }
}