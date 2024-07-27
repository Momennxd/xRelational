using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_Engine
{
    public class clsExceptions
    {


        public class TableNameDoesNotExistException : Exception
        {
            public TableNameDoesNotExistException(string message) : base(message)
            {
                TableName = message;
            }

            public string TableName { get; set; }

        }

        public class ColumnNameDoesNotExistException : Exception
        {
            public ColumnNameDoesNotExistException(string message) : base(message)
            {
                ColumnName = message;
            }

            public string ColumnName { get; set; }

        }

        public class TableNameExistsException : Exception
        {
            public TableNameExistsException(string message) : base(message)
            {
                TableName = message;
            }

            public string TableName { get; set; }


        }

        public class ColumnValuesEqualityException : Exception
        {
            public ColumnValuesEqualityException(string message) : base(message)
            {
                ColumnName = message;
            }

            public string ColumnName { get; set; }


        }

        public class PrimaryKeyExistsException : Exception
        {
            public PrimaryKeyExistsException(string message) : base(message)
            {
                PrimaryKey = message;
            }

            public string PrimaryKey { get; set; }


        }

        public class PrimaryKeyDoesNotExistsException : Exception
        {
            public PrimaryKeyDoesNotExistsException(string message) : base(message)
            {
                PrimaryKey = message;
            }

            public string PrimaryKey { get; set; }


        }

        public class NegtivePrimaryKeyException : Exception
        {
            public NegtivePrimaryKeyException(string message) : base(message)
            {
                PrimaryKey = message;
            }

            public string PrimaryKey { get; set; }


        }

        public class UniqeValExistsException : Exception
        {
            public UniqeValExistsException(string ColumnName, string columnValue) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
                this.ColumnValue = columnValue;
            }

            public string ColumnName { get; set; }

            public string ColumnValue { get; set; }

        }

        public class ColumnNullValue : Exception
        {
            public ColumnNullValue(string ColumnName) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
            }

            public string ColumnName { get; set; }


        }

        public class EmptyColumnVal : Exception
        {
            public EmptyColumnVal(string ColumnName) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
            }

            public string ColumnName { get; set; }


        }

        public class InvalidType : Exception
        {
            public InvalidType(string ColumnName, Type ColumnType) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
                this.ColumnType = ColumnType;

            }

            public string ColumnName { get; set; }

            public Type ColumnType { get; set; }


        }

        public class UniqeNullValueException : Exception
        {
            public UniqeNullValueException(string ColumnName) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
            }

            public string ColumnName { get; set; }


        }


        public class StringFormatException : Exception
        {
            public StringFormatException(string ColumnName) : base(ColumnName)
            {
                this.ColumnName = ColumnName;
            }

            public string ColumnName { get; set; }


        }

    }
}
