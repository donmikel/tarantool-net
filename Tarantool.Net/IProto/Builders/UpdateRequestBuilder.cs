using System;
using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class UpdateRequestBuilder : RequestBuilder
    {
        private readonly UpdateRequest _updateRequest;

        public UpdateRequestBuilder(UpdateRequest updateRequest) : base(updateRequest)
        {
            _updateRequest = updateRequest;
        }

        /// <summary>
        /// Set Space Id
        /// </summary>
        /// <param name="spaceId">Space Id</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithSpaceId(UInt32 spaceId)
        {
            _updateRequest.SpaceId = spaceId;
            return this;
        }

        /// <summary>
        /// Set Index Id
        /// </summary>
        /// <param name="indexId">Index Id</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithIndexId(UInt32 indexId)
        {
            _updateRequest.IndexId = indexId;
            return this;
        }

        /// <summary>
        /// Set Key Tuple
        /// </summary>
        /// <param name="keys">Key tuple</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithKeys(object[] keys)
        {
            _updateRequest.Key = new Tuple(keys);
            return this;
        }

        /// <summary>
        /// Set Key Tuple
        /// </summary>
        /// <param name="keyTupleBuilder">Key Builder</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithKeys(Func<TupleBuilder, TupleBuilder> keyTupleBuilder)
        {
            var ktb = new TupleBuilder(_updateRequest.Key);
            _updateRequest.Key = keyTupleBuilder(ktb);
            return this;
        }

        /// <summary>
        /// Set Key Tuple
        /// </summary>
        /// <param name="key">Key tuple</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithKey(object key)
        {
            return WithKeys(t => t.AddField(key));
        }

        /// <summary>
        /// Set Update Operations
        /// </summary>
        /// <param name="operationTupleBuilder">Create Operation Tuple Action</param>
        /// <returns></returns>
        public UpdateRequestBuilder WithOperation(Func<UpdateOperationsTupleBuilder, UpdateOperationsTupleBuilder> operationTupleBuilder)
        {
            var uotp = new UpdateOperationsTupleBuilder(_updateRequest.UpdateOperations);
            _updateRequest.UpdateOperations = operationTupleBuilder(uotp);
            return this;
        }

        public override RequestBase Build()
        {
            WithHeader(Command.UPDATE)
                .WithInstruction(Key.SPACE, _updateRequest.SpaceId)
                .WithInstruction(Key.INDEX, _updateRequest.IndexId)
                .WithInstruction(Key.KEY, _updateRequest.Key)
                .WithTuple(_updateRequest.UpdateOperations);

            return this;
        }

        public static implicit operator UpdateRequest(UpdateRequestBuilder updateRequestBuilder)
        {
            return updateRequestBuilder._updateRequest;
        }
    }
}