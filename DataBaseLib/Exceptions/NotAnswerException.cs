namespace DataBaseLib.Exceptions
{
    public class NotAnswerException: DataBaseException
    {
        public NotAnswerException(TypeException typeException, string tableName) : base(typeException, tableName)
        {
        }

        public NotAnswerException(string message, TypeException typeException, string tableName) : base(message, typeException, tableName)
        {
        }

    }
}
