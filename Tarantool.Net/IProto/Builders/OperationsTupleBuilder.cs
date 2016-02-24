namespace Tarantool.Net.IProto.Builders
{
    public class OperationsTupleBuilder
    {
        protected readonly OperationsTuple _tuple;
        public OperationsTupleBuilder(OperationsTuple tuple)
        {
            _tuple = tuple;
        }

        public OperationsTupleBuilder Addition(int fieldNumber, int argument)
        {
            return AddOperation(UpdateOperationCode.Addition, fieldNumber, argument);
        }

        public OperationsTupleBuilder Subtraction(int fieldNumber, int argument)
        {
            return AddOperation(UpdateOperationCode.Subtraction, fieldNumber, argument);
        }

        public OperationsTupleBuilder Delete<T>(int fieldNumber, T argument)
        {
            return AddOperation(UpdateOperationCode.Delete, fieldNumber, argument);
        }

        public OperationsTupleBuilder Insert<T>(int fieldNumber, T argument)
        {
            return AddOperation(UpdateOperationCode.Insert, fieldNumber, argument);
        }

        public OperationsTupleBuilder Assign<T>(int fieldNumber, T argument)
        {
            return AddOperation(UpdateOperationCode.Assign, fieldNumber, argument);
        }

        protected OperationsTupleBuilder AddOperation<T>(string operationType, int fieldNumber, T argument)
        {
            _tuple.AddField(new UpdateOperation<T>
            {
                Code = operationType,
                FieldNo = (uint)fieldNumber,
                Arg = argument
            });

            return this;
        }

        public static implicit operator OperationsTuple(OperationsTupleBuilder tupleBuilder)
        {
            return tupleBuilder._tuple;
        }
    }
}