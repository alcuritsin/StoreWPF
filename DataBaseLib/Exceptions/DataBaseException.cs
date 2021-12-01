#nullable enable
using System;


namespace DataBaseLib.Exceptions
{
    public abstract class DataBaseException : Exception
    {
        public TypeException TypeException { get; protected set; }
        public string TableName { get; protected set; }

        protected DataBaseException(TypeException typeException, string tableName)
        {
            TypeException = typeException;
            TableName = tableName;
        }

        protected DataBaseException(string? message, TypeException typeException, string tableName) : base(message)

        {
            TypeException = typeException;
            TableName = tableName;
        }
    }

}