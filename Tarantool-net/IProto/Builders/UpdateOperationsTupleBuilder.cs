using Tarantool.Net.IProto.Requests;

namespace Tarantool.Net.IProto.Builders
{
    public class UpdateOperationsTupleBuilder : OperationsTupleBuilder
    {

        public UpdateOperationsTupleBuilder(OperationsTuple tuple) : base(tuple)
        {
        }

        /// <summary>
        /// Addition OP='+' .space[<see cref="UpdateRequest.Key"/>][<see cref="fieldNumber"/>] += <see cref="argument"/>
        /// </summary>
        /// <param name="fieldNumber">Field Number</param>
        /// <param name="argument">Argument</param>
        /// <returns></returns>
        public new UpdateOperationsTupleBuilder Addition(int fieldNumber, int argument)
        {
            return (UpdateOperationsTupleBuilder)base.Addition(fieldNumber, argument);
        }

        /// <summary>
        /// Subtraction OP='-' .space[<see cref="UpdateRequest.Key"/>][<see cref="fieldNumber"/>] -= <see cref="argument"/>
        /// </summary>
        /// <param name="fieldNumber">Field Number</param>
        /// <param name="argument">Argument</param>
        /// <returns></returns>
        public UpdateOperationsTupleBuilder Subtraction(int fieldNumber, int argument)
        {
            return (UpdateOperationsTupleBuilder)base.Subtraction(fieldNumber, argument);
        }

        /// <summary>
        /// Bitwise AND OP='&' .space[<see cref="UpdateRequest.Key"/>][<see cref="fieldNumber"/>] &= <see cref="argument"/>
        /// </summary>
        /// <param name="fieldNumber">Field Number</param>
        /// <param name="argument">Argument</param>
        /// <returns></returns>
        public UpdateOperationsTupleBuilder BitwiseAnd(int fieldNumber, int argument)
        {
            return AddOperation(UpdateOperationCode.BitwiseAnd, fieldNumber, argument);
        }

        // ReSharper disable once InconsistentNaming
        public UpdateOperationsTupleBuilder BitwiseXOR(int fieldNumber, int argument)
        {
            return AddOperation(UpdateOperationCode.BitwiseXOR, fieldNumber, argument);
        }

        public UpdateOperationsTupleBuilder BitwiseOr(int fieldNumber, int argument)
        {
            return AddOperation(UpdateOperationCode.BitwiseOr, fieldNumber, argument);
        }

        /// <summary>
        /// Delete <see cref="argument"/> fields starting from <see cref="fieldNumber"/> in the space[<see cref="UpdateRequest.Key"/>]
        /// </summary>
        /// <param name="fieldNumber"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public new UpdateOperationsTupleBuilder Delete<T>(int fieldNumber, T argument)
        {
            return (UpdateOperationsTupleBuilder)base.Delete(fieldNumber, argument);
        }

        public new UpdateOperationsTupleBuilder Insert<T>(int fieldNumber, T argument)
        {
            return (UpdateOperationsTupleBuilder)base.Insert(fieldNumber, argument);
        }

        public new UpdateOperationsTupleBuilder Assign<T>(int fieldNumber, T argument)
        {
            return (UpdateOperationsTupleBuilder)base.Assign(fieldNumber, argument);
        }

        private new UpdateOperationsTupleBuilder AddOperation<T>(string operationType, int fieldNumber, T argument)
        {
            return (UpdateOperationsTupleBuilder)base.AddOperation(operationType, fieldNumber, argument);
        }

        /// <summary>
        /// Splice OP=':' take the string from space[<see cref="UpdateRequest.Key"/>][<see cref="fieldNumber"/>] and substitute <see cref="offset"/> bytes from <see cref="possition"/> with <see cref="argument"/>
        /// </summary>
        /// <param name="fieldNumber">Field Number</param>
        /// <param name="offset">Offset</param>
        /// <param name="argument">Argument</param>
        /// <param name="possition">Possition</param>
        /// <returns></returns>
        public UpdateOperationsTupleBuilder Splice(int fieldNumber, int possition, int offset, string argument)
        {
            _tuple.AddField(new SpliceUpdateOperation
            {
                Offset = (uint)offset,
                Position = (uint)possition,
                FieldNo = (uint)fieldNumber,
                Arg = argument
            });
            return this;
        }

        public static implicit operator OperationsTuple(UpdateOperationsTupleBuilder tupleBuilder)
        {
            return tupleBuilder._tuple;
        }
    }
}