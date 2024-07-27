
using XRelational_DataAccess;

namespace XRelational_Engine
{
    public class clsTable
    {

        public enum enMode { eAdd, eUpdate }

        private enMode Mode = enMode.eUpdate;


        public string TableName { get; private set; }

        public string DataBaseName { get; private set; }

        public string PK_ColumnName
        { 
            get
            { 
                return clsTable.GetPrimaryKeyColumnName(this.TableName, this.DataBaseName);
            }
        }
        
        public long RowsCount { get {  return _GetRowsCount(); } }


        private clsTable()
        {

        }







        //public methods



        public void AddRow(clsDataRow row)
        {

            if (row == null)
                return;

            row.CompleteRow(TableName, DataBaseName);

            clsTableDA.AddRow(DataBaseName, TableName, row.ROW);

        }


        public static clsTable Find(string TableName, string DatabaseName)
        {

            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DatabaseName))
                return null;

            //setting the table values
            clsTable table = new clsTable();
            try
            {
                table.DataBaseName = DatabaseName;

                //validation
                table._SetTableName(TableName);
            }
            catch
            {
                return null;
            }
          
            return table;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DbName"></param>
        /// <returns>
        /// the primary key column name
        /// <para></para>
        /// NULL if it does not exist
        /// </returns>
        public static string GetPrimaryKeyColumnName(string TableName, string DbName)
        {
            return clsTableDA.GetPrimaryKeyColumnName(TableName, DbName);
        }
















        //private methods   

        private long _GetRowsCount()
        {
            return clsTableGlob.GetRowsCount(this.TableName, this.DataBaseName);
        }

        /// <summary>
        /// this function sets the table name and it makes sure that the table exist in the data base
        /// </summary>
        /// <param name="TableName"></param>
        /// <exception cref="clsExceptions.TableNameDoesNotExistException">
        /// thrown when the table does NOT exist
        /// </exception>
        private void _SetTableName(string TableName)
        {
            if (clsTableDA.DoesTableExist(TableName, DataBaseName))
            {
                this.TableName = TableName;
            }
            else
            {
                if (TableName != this.TableName)
                    throw new clsExceptions.TableNameDoesNotExistException(TableName);
            }
        }





    }
}
