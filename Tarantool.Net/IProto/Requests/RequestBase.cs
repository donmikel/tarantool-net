using System;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Net.IProto.Requests
{
    public abstract class RequestBase
    {
        private static readonly MessagePackSerializer<RequestBody> Serializer = SerializationContext.Default.GetSerializer<RequestBody>();
        internal PacketHeader Header;
        internal readonly RequestBody Body = new RequestBody();

        public RequestBase WithSync(int requestId)
        {
            Header = new PacketHeader(Header.Command, requestId, Header.SchemaId);
            return this;
        }

        public byte[] GetBytes()
        {
            SerializationContext.Default.CompatibilityOptions.PackerCompatibilityOptions =
                PackerCompatibilityOptions.PackBinaryAsRaw;

            var header = Header.Raw;
            var body = Serializer.PackSingleObject(Body);

            var totalLen = body.Length + header.Length;
            var resArray = new byte[totalLen + 5];
            var prefix = new byte[] { 0xCE };
            prefix.CopyTo(resArray, 0);

            resArray[1] = (byte)(totalLen >> 24);
            resArray[2] = (byte)(totalLen >> 16);
            resArray[3] = (byte)(totalLen >> 8);
            resArray[4] = (byte)totalLen;
            header.CopyTo(resArray, 5);
            var offcet = 5 + header.Length;
            Array.Copy(body, 0, resArray, offcet, body.Length);

            return resArray;
        }

    }

    public abstract class RequestBuilder
    {
        private readonly RequestBase _replaceRequest;

        protected RequestBuilder(RequestBase replaceRequest)
        {
            _replaceRequest = replaceRequest;
        }

        protected internal RequestBuilder WithHeader(Command commmand, int schemaId = 0)
        {
            _replaceRequest.Header = new PacketHeader(commmand, 0, schemaId);
            return this;
        }

        protected internal RequestBuilder WithInstruction<T>(Key key, T value)
        {
            _replaceRequest.Body.AddBodyData(key, value);
            return this;
        }

        protected internal RequestBuilder WithTuple(Tuple tuple)
        {
            _replaceRequest.Body.AddBodyData(Key.TUPLE, tuple);
            return this;
        }

        public abstract RequestBase Build();

    }
}