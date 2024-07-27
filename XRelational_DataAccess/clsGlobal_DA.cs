using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRelational_DataAccess
{
    public class clsGlobal_DA
    {


        public static bool SortDatatable(ref DataTable dtROWS, string CI_ColumnName)
        {
            if (dtROWS == null || string.IsNullOrEmpty(CI_ColumnName) || dtROWS.Rows.Count == 0) return false;

            try
            {
                long.Parse(dtROWS.Rows[0][CI_ColumnName].ToString());
            }
            catch
            {
                return false;
            }



            SortedDictionary<long, DataRow> RowskeyValuePairs = new SortedDictionary<long, DataRow>();
            //adding the rows to the dictionary
            foreach (DataRow row in dtROWS.Rows)
            {
                RowskeyValuePairs.Add(Convert.ToInt64(row[CI_ColumnName]), row);
            }


            dtROWS = RowskeyValuePairs.Values.CopyToDataTable();
            return true;
        }


        /// <summary>
        /// this function gets the columns list for an existed table 
        /// 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>
        /// NULL if the table does not exist.
        /// DataColumnCollection of the table columns
        /// 
        /// </returns>
        public static DataColumnCollection GetColumnsList(string xmlFilePath)
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet.ReadXml(xmlFilePath);
                //getting the second table which has the columns list
                return dataSet.Tables[1].Columns;
            }
            catch
            {
                return null;
            }

        }

        public static DataTable UnionDataTables(ref List<DataTable> tables)
        {

            if (tables == null)
                return null;

            if (tables.Count == 1)
                return tables[0];

            // Clone the structure of the first DataTable to the new DataTable
            DataTable result = tables[0].Clone();

            // Import rows from each DataTable
            foreach (DataTable table in tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    result.ImportRow(row);
                }
            }

            return result;
        }

        /// <summary>
        /// this functions filters the primary key in a data table 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MainTable"></param>
        /// <param name="FilterValue"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public static DataTable FilterDataTable<T>(DataTable MainTable, List<T> FilterValue, string ColumnName)
        {
            DataTable ResultSet = MainTable.Clone();

            foreach(DataRow row in MainTable.Rows)
            {
                foreach(T FilterVal in FilterValue)
                {
                    DataRow _row = ResultSet.NewRow();

                    if (row[ColumnName].ToString() == FilterVal.ToString())
                    {
                        //copying all the columns form the main row to the result set row
                        _row.ItemArray = row.ItemArray;
                        ResultSet.Rows.Add(_row);
                        break;
                    }
                }
            }


            return ResultSet;
        }

        public static DataTable FilterColumns(DataTable originalTable, string[] desiredColumns)
        {
             // Create a DataView from the original DataTable
             DataView dv = new DataView(originalTable);

            // Use the ToTable method to create a new DataTable with the desired columns
            return dv.ToTable(false, desiredColumns);

        }


        /// <summary>
        /// this function filters a table based on a dictionary that hold the column name and its value
        /// in a nested dictionary and the value is if the column value should be matched to valid the row
        /// it is used with AND and OR operator
        /// </summary>
        /// <param name="dtDataPageTable"></param>
        /// <param name="dcFilteringColumns"></param>
        /// <returns></returns>
        public static DataTable FilterTable(DataTable dtDataPageTable,
            Dictionary<Dictionary<string, string>, bool> dcFilteringColumns)
        {

            if (dcFilteringColumns.Count == 0)
            {
                return dtDataPageTable;
            }

            DataTable ResultSet = dtDataPageTable.Clone();
            bool IsValid;

          

            foreach (DataRow DataPageROW in dtDataPageTable.Rows)
            {
                IsValid = false;
                DataRow _ReseltSetROW = ResultSet.NewRow();
                foreach (var FilterColumn in dcFilteringColumns)
                {
                    _ReseltSetROW = ResultSet.NewRow();

                    //getting the columns name and data from the first nested dic in the main dcFilteringColumns
                    string ColumnName = FilterColumn.Key.ElementAt(0).Key.ToString();
                    string ColumnValue = FilterColumn.Key.ElementAt(0).Value.ToString();

                    if (DataPageROW[ColumnName].ToString() == ColumnValue.ToString())
                    {
                        IsValid = true;                     
                    }
                    else
                    {
                        //if it is a must to match
                        if (FilterColumn.Value)
                        {
                            IsValid = false;
                            break;

                        }
                    }

                }


                if (IsValid)
                {
                    //copying all the columns form the main row to the result set row
                    _ReseltSetROW.ItemArray = DataPageROW.ItemArray;
                    ResultSet.Rows.Add(_ReseltSetROW);
                }


            }

            return ResultSet;

        }


        public static DataTable GetOverlappingRows(DataTable table1, DataTable table2)
        {
            var overlappingRows = 
                table1.AsEnumerable().Intersect(table2.AsEnumerable(), DataRowComparer.Default);
            DataTable result = table1.Clone(); // Create new table with same schema

            foreach (DataRow row in overlappingRows)
            {
                result.ImportRow(row);
            }

            return result;
        }

        

        public static List<string> GetColumnsNamesList(string TableName, string DatabaseName)
        {
            DataSet dataSet = clsXML.XmlToDataSet(clsPaths.GetTableStructureFilePath(DatabaseName, TableName));
            List<string> ColumnsNames = new List<string>();

            foreach (DataTable ds in dataSet.Tables)
            {
                //skipping the first table which is the table info (ID, name, database name..)
                if (ds.TableName != TableName)
                    ColumnsNames.Add(ds.TableName);
            }

            return ColumnsNames;
        }


        public static bool DoesPrimaryKeyExist(long PK, string PkColumnName, string dbName, string TableName)
        {
            string Path = clsPaths.GetExistDataPagePath_CI(dbName, TableName, PK);

            if (Path == null)
                return false;


            DataTable dtPageTable = clsXML.XmlToDataTable(Path, 1);

            if (dtPageTable == null) return false;

            foreach (DataRow dr in dtPageTable.Rows)
            {
                try
                {
                    if (Convert.ToInt64(dr[PkColumnName]) == PK)
                    {
                        return true;
                    }
                }
                catch
                {
                    //pk column name does not exist
                    return false;
                }

            }

            return false;

        }


        public static bool DoesColumnValExist(string Value, string ColumnName, string dbName, string TableName)
        {

            DataTable dtTableData = clsTableDA.SelectTable(dbName, TableName, new List<string> { ColumnName });

            if (dtTableData == null)
                return false;


            foreach(DataRow dr in dtTableData.Rows)
            {
                if (dr[ColumnName].Equals(Value))
                {
                    return true;
                }
            }


            return false;
        }


        public static long GetPrimaryKeyValue(ref Dictionary<clsColumnInfo, object> ROW)
        {
            foreach(var column in ROW.Keys)
            {
                if (column.PK)
                    return Convert.ToInt64(ROW[column]);
            }


            return -1;
        }


        public static void FilterTableColumns(ref DataTable dt, List<string> ResultColumnsNames)
        {

            foreach (DataColumn column in dt.Columns)
            {
                if (!ResultColumnsNames.Contains(column.ColumnName))
                {
                    dt.Columns.Remove(column);
                }
            }
            


        }

    }
}
