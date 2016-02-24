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
            MessagePackObject obj;

            while (unpacker.ReadObject(out obj))
            {
                if (obj.IsDictionary)
                {
                    var dict = obj.AsDictionary();
                    if (dict.Keys.Count > 0)
                    {
                        foreach(var key in dict.Keys)
                        {
                            var code = key.AsInt32();

                            switch (code)
                            {
                                case (int)Key.CODE:
                                    Code = dict[key].AsInt32();
                                    break;
                                case (int)Key.SYNC:
                                    Sync = dict[key].AsInt32();
                                    break;
                                case (int)Key.SCHEMA_ID:
                                    SchemaId = dict[key].AsInt32();
                                    break;
                                case (int)Key.DATA:
                                    Body = dict[key].AsList().Select(i => new Tuple(i.AsList().Select(a => a.ToObject()).ToList())).ToList();
                                    break;
                                case (int)Key.ERROR:
                                    Error = dict[key].AsString();
                                    IsError = true;
                                    break;
                            }
                        }
                    }
                }

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