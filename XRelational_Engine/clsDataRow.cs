using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRelational_DataAccess;

namespace XRelational_Engine
{
    public  class clsDataRow
    {

        internal Dictionary<clsColumnInfo, object> ROW { get; private set; }

        public struct stColumnInfo
        {
            public string ColumnName { get; set; }

            public bool Nullable { get; set; }

            public bool PK { get; set; }

            public bool Uniqe { get; set; }

            public string type { get; set; }


        }




        public clsDataRow()
        {

            ROW = new Dictionary<clsColumnInfo, object>();
        }




        public bool AddColumn(stColumnInfo ColumnInfo, object ColumnValue)
        {

            try
            {
                ROW.Add(new clsColumnInfo(ColumnInfo.ColumnName, ColumnInfo.Nullable,
                    ColumnInfo.PK, ColumnInfo.Uniqe, clsColumnInfo.GetColumnType(ColumnInfo.type)), ColumnValue);

                    return true;
            }
            catch
            {
                return false;
            }

        }


        internal bool AddColumn(clsColumnInfo ColumnInfo, object ColumnValue)
        {

            try
            {
                ROW.Add(ColumnInfo, ColumnValue);

                return true;
            }
            catch
            {
                return false;
            }

        }




        /// <summary>
        /// completes the missing columns by getting the columns list from the table and add them to the row
        /// with NULL value
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DbName"></param>
        public void CompleteRow(string TableName, string DbName)
        {

            List<string> ColumnsNames = clsTableDA.GetColumnsList(TableName, DbName);


            foreach (string ColumnName in  ColumnsNames)
            {

                bool ColumnExist = false;
                foreach (var key in ROW.Keys)
                {

                    if (key.ColumnName == ColumnName)
                    {
                        ColumnExist = true;
                        break;
                    }

                }

                if (!ColumnExist)
                {
                    ROW.Add(clsTableDA.GetColumnsInfo(TableName, DbName, ColumnName), null);
                }

            }

            


        }


    }
}
