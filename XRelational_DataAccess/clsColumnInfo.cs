using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_DataAccess
{
    //has a switch case for types
    public class clsColumnInfo
    {

        public enum enColumnType
        {
            BYTE, INT, LONG, FLOAT, DOUBLE,
            STRING, 
            DATETIME,
            BOOL,
            NULL

        }

        public string ColumnName { get; set; }

        public bool Nullable { get; set; }

        public bool PK { get; set; }

        public bool Uniqe { get; set; }

        public enColumnType type { get; set; }
            


        public clsColumnInfo(string columnName, bool nullable, bool pK, bool uniqe, enColumnType type)
        {
            ColumnName = columnName;
            Nullable = nullable;
            PK = pK;
            Uniqe = uniqe;
            this.type = type;
        }

        public clsColumnInfo() {

            this.ColumnName = "";
            this.Nullable = false;
            this.PK = false;
            this.Uniqe = false;
            this.type = enColumnType.NULL;


        }

        public static enColumnType GetColumnType(string sType)
        {
            switch (sType.ToLower())
            {
                case "byte":
                    return enColumnType.BYTE;
                case "int":
                    return enColumnType.INT;
                case "long":
                    return enColumnType.LONG;
                case "float":
                    return enColumnType.FLOAT;
                case "double":
                    return enColumnType.DOUBLE;
                case "string":
                    return enColumnType.STRING;
                case "datetime":
                    return enColumnType.DATETIME;
                case "bool":
                    return enColumnType.BOOL;
                case "null":
                    return enColumnType.NULL;

                default:
                    return enColumnType.NULL;
            }
        }

        public static Type GetColumnType(enColumnType type)
        {
            switch (type)
            {
                case enColumnType.BYTE:
                    return typeof(byte);

                case enColumnType.INT:
                    return typeof(int);

                case enColumnType.LONG:
                    return typeof(long);

                case enColumnType.FLOAT:
                    return typeof(float);

                case enColumnType.DOUBLE:
                    return typeof(double);

                case enColumnType.STRING:
                    return typeof(string);

                case enColumnType.DATETIME:
                    return typeof(DateTime);

                case enColumnType.BOOL:
                    return typeof(bool);

                default:
                    return typeof(string);
            }


        }

      

    }
}
