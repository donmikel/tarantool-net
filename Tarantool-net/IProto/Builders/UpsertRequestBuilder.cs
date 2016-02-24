using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class UpsertRequestBuilder : RequestBuilder
    {
        private readonly UpsertRequest _upsertRequest;

        public UpsertRequestBuilder(UpsertRequest upsertRequest) : base(upsertRequest)
        {
            _upsertRequest = upsertRequest;
        }

        /// <summary>
        /// Set Space Id
        /// </summary>
        /// <param name="spaceId">Space Id</param>
        /// <returns></returns>
        public UpsertRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _upsertRequest.SpaceId = spaceId;
            return this;
        }

        /// <summary>
        /// Set Tuple
        /// </summary>
        /// <param name="tuple">Tuple</param>
        /// <returns></returns>
        public UpsertRequestBuilder WithTuple(object[] tuple)
        {
            _upsertRequest.Tuple = new Tuple(tuple);
            return this;
        }

        /// <summary>
        /// Set Tuple
        /// </summary>
        /// <param name="tupleBuilder">Key Builder</param>
        /// <returns></returns>
        public UpsertRequestBuilder WithTuple(Func<TupleBuilder, TupleBuilder> tupleBuilder)
        {
            var tb = new TupleBuilder(_upsertRequest.Tuple);
            _upsertRequest.Tuple = tupleBuilder(tb);
            return this;
        }

        /// <summary>
        /// Set Tuple
        /// </summary>
        /// <param name="field">One element tuple</param>
        /// <returns></returns>
        public UpsertRequestBuilder WithTuple(object field)
        {
            return WithTuple(t => t.AddField(field));
        }

        public UpsertRequestBuilder WithOperation(Func<OperationsTupleBuilder, OperationsTupleBuilder> operationTupleBuilder)
        {
            var uotp = new OperationsTupleBuilder(_upsertRequest.UpsertOperations);
            _upsertRequest.UpsertOperations = operationTupleBuilder(uotp);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.UPSERT)
                .WithInstruction(Key.SPACE, _upsertRequest.SpaceId)
                .WithTuple(_upsertRequest.Tuple)
                .WithInstruction(Key.UPSERT_OPS, _upsertRequest.UpsertOperations);

            return this;
        }

        public static implicit operator UpsertRequest(UpsertRequestBuilder upsertRequestBuilder)
        {
            return upsertRequestBuilder._upsertRequest;
        }
    }
}