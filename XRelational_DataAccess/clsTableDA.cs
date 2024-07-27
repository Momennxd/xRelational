using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRelational_Engine;
using static XRelational_DataAccess.clsColumnInfo;

namespace XRelational_DataAccess
{
    //has a switch case for types
    public class clsTableDA
    {
        public struct st_Table_InfoDA
        {

            public int TableID { get; set; }

            public string TableName { get; set; }

            public string PK_ColumnName { get; set; }
        }




        /// <summary>
        /// the function loops through the targeted table folder then gets all the xml files
        /// and converts them to data tabels
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static List<DataTable> _GetTargetTablesList(string folderPath)
        {
            List<DataTable> lstTables = new List<DataTable>();

            // Check if the directory exists
            if (Directory.Exists(folderPath))
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(folderPath);

                // Loop through each file
                foreach (string file in files)
                {
                    lstTables.Add(clsXML.XmlToDataTable(file, 1));
                }
            }
            else
                return null;

            return lstTables;

        }

        /// <summary>
        /// filtering and unioning tables list
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        private static DataTable _GetUnionedTargetTable(string DatabaseName, string TableName)
        {
            //looping through all xml files in the table folder 
            List<DataTable> lstTables =
                _GetTargetTablesList(clsPaths.GetTableDataPagesFolderPath(DatabaseName, TableName));


            return clsGlobal_DA.UnionDataTables(ref lstTables);

        }


        private static clsColumnInfo _GetColumnInfo(DataTable dtColumnInfo)
        {
            clsColumnInfo columnInfo = new clsColumnInfo();

            columnInfo.ColumnName = dtColumnInfo.TableName;

            foreach (DataRow row in dtColumnInfo.Rows)
            {
                foreach (DataColumn column in dtColumnInfo.Columns)
                {


                    switch (column.ColumnName.ToLower())
                    {
                        case "null":
                            columnInfo.Nullable = Convert.ToBoolean(row[column]);
                            break;

                        case "pk":
                            columnInfo.PK = Convert.ToBoolean(row[column]);
                            break;

                        case "uniqe":
                            columnInfo.Uniqe = Convert.ToBoolean(row[column]);
                            break;
                        case "type":
                            columnInfo.type = clsColumnInfo.GetColumnType(row[column].ToString());
                            break;
                    }


                }
            }

            return columnInfo;
        }

        private static bool ValidateColumnType(KeyValuePair<clsColumnInfo, object> column)
        {


            switch (column.Key.type)
            {
                case enColumnType.BYTE:
                    return byte.TryParse(column.Value.ToString(), out byte outByte);

                case enColumnType.INT:
                    return int.TryParse(column.Value.ToString(), out int outInt);

                case enColumnType.LONG:
                    return long.TryParse(column.Value.ToString(), out long outLong);

                case enColumnType.FLOAT:
                    return float.TryParse(column.Value.ToString(), out float outFloat);

                case enColumnType.DOUBLE:
                    return double.TryParse(column.Value.ToString(), out double outDouble);

                case enColumnType.STRING:
                    return column.Value.GetType() == typeof(string);

                case enColumnType.DATETIME:
                    return DateTime.TryParse(column.Value.ToString(), out DateTime outDate);

                case enColumnType.BOOL:
                    return bool.TryParse(column.Value.ToString(), out bool outBool);

                default:
                    return false;
            }

        }


        private static void _ValidateRow(string DatabaseName, string TableName, Dictionary<clsColumnInfo, object> ROW)
        {
            foreach (var Column in ROW.Keys)
            {

                //validating if value is NULL
                if (!Column.Nullable)
                {
                    if (ROW[Column] == null)
                        throw new clsExceptions.ColumnNullValue(Column.ColumnName);
                }

                else
                {

                    if (Column.Uniqe)
                    {
                        if (ROW[Column] == null)
                            throw new clsExceptions.UniqeNullValueException(Column.ColumnName);
                    }
                    else
                    {
                        if (ROW[Column] == null)
                            continue;
                    }



                }


                if (Column.PK)
                {

                    if (Convert.ToInt64(ROW[Column]) <= 0)
                    {
                        throw new clsExceptions.NegtivePrimaryKeyException(Column.PK.ToString());
                    }

                    if (clsGlobal_DA.DoesPrimaryKeyExist(Convert.ToInt64(ROW[Column]),
                        Column.ColumnName, DatabaseName, TableName))
                    {
                        throw new clsExceptions.PrimaryKeyExistsException(Column.PK.ToString());

                    }

                }



                if (ROW[Column] != null && ROW[Column].ToString() == string.Empty)
                {
                    throw new clsExceptions.EmptyColumnVal(Column.ColumnName);
                }


                if (Column.Uniqe && !Column.PK)
                {
                    if (clsGlobal_DA.DoesColumnValExist(ROW[Column].ToString(),
                        Column.ColumnName, DatabaseName, TableName))
                    {
                        throw new clsExceptions.UniqeValExistsException(Column.ColumnName, ROW[Column].ToString());
                    }
                }


                if (!ValidateColumnType(new KeyValuePair<clsColumnInfo, object>(Column, ROW[Column])))
                {
                    throw new clsExceptions.InvalidType(Column.ColumnName, ROW[Column].GetType());
                }



            }

        }

        private static string _CreateEmptyDataPage(string PageName, string DatabaseName, string TableName)
        {
            string DataPagesPath = clsPaths.GetTableDataPagesFolderPath(DatabaseName, TableName);

            string PagePath = clsPaths.GetDataPagePath(DatabaseName, TableName, PageName);

            clsXML.CreateXmlFile(DataPagesPath, PageName, TableName);

           // clsXML.AddXmlElement(TableName, 0, "Person", "1", PagePath);

            clsXML.AddXmlAttribute("TableName", TableName, PagePath);
            clsXML.AddXmlAttribute("DataBaseName", DatabaseName, PagePath);

            return PagePath;

        }
     

        private static string GetDataPagePathToInsert(long PK, string DatabaseName, string TableName)
        {
            //it means there is no CI
            if (PK == -1)
            {
                string DataPgesPath = clsPaths.GetTableDataPagesFolderPath(DatabaseName, TableName);

                if (Directory.Exists(DataPgesPath))
                {
                    string[] Files = Directory.GetFiles(DataPgesPath);

                    if (Files.Length == 0)
                    {
                        return _CreateEmptyDataPage(clsConst.dataPage_MainPageName, DatabaseName, TableName);                       
                    }
                    else
                    {
                        //since there has to be 1 xml file when there is no CI
                        return Files[0];
                    }

                }
                else
                    return null;

            }

            string PageName = clsPaths.GetDataPageName(PK);
            string __PagePath = clsPaths.GetDataPagePath(DatabaseName, TableName, PageName);

            // Check if the file exists
            if (!File.Exists(__PagePath))
            {
                return _CreateEmptyDataPage(PageName, DatabaseName, TableName);
            }


            return __PagePath;

        }


        private static bool _AddRowToPage(bool CI_AddMethod, string PagePath, string TableName, string DbName,
            Dictionary<clsColumnInfo, object> ROW)
        {

            if (string.IsNullOrEmpty(PagePath) || string.IsNullOrEmpty(TableName) ||
                 string.IsNullOrEmpty(DbName) || ROW == null)
            {
                return false;
            }


            //adding in a normal way (adding to the main page) if there is NO PK 
            if (!CI_AddMethod)
            {

                clsXML.AddXmlElementToRoot(clsConst.xml_DataPageElementName, string.Empty, PagePath);
                //add the elements to the bottom of the xml file

                //setting the row
                foreach (var key in ROW.Keys)
                {
                    clsXML.AddXmlsubElementToLastElement(clsConst.xml_DataPageElementName,
                        key.ColumnName, ROW[key].ToString(), PagePath);
                }

                return true ;
            }


            List<string> tableColumnsNames = GetColumnsList(TableName, DbName); 


            DataTable dtXmlRows = clsXML.XmlToDataTable(PagePath, 1);
            //if null means that the page is empty so, the for loop is to set the tabel structure
            if (dtXmlRows == null)
            {
                dtXmlRows = new DataTable();
                tableColumnsNames.ForEach(cName => dtXmlRows.Columns.Add(new DataColumn(cName, typeof(string))));
            }


            DataRow dr = dtXmlRows.NewRow();
            //setting the row
            foreach (var key in ROW.Keys)
            {
                dr[key.ColumnName] = ROW[key] == null ? "NULL" : ROW[key];
            }

            dtXmlRows.Rows.Add(dr);


            clsGlobal_DA.SortDatatable(ref dtXmlRows, GetPrimaryKeyColumnName(TableName, DbName));

            clsXML.ClearXmlFile(PagePath);

            clsXML.InsertDataTableToXml(ref dtXmlRows, PagePath, tableColumnsNames);


            return true;

        }

        private static void _IncreaseRowCount(string DatabaseName, string TableName)
        {


            string Path = clsPaths.GetTableGlobeFilePath(DatabaseName, TableName);

      
            long RowsCount = Convert.ToInt64(clsXML.GetElementValue(Path, clsConst.RowsCountNameInTablGlobe));
            RowsCount++;

            clsXML.UpdateElementValue(Path, clsConst.RowsCountNameInTablGlobe, RowsCount.ToString());


        }








        //TABLE FUNCTIONS----------------------------------------------------------

        public static bool DoesTableExist(string TableName, string DatabaseName)
        {

            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DatabaseName))
            { return false; }

             string TablePath =
                $@"{clsPaths.DatabasesPath}\{DatabaseName}\table_{TableName}";


            return Directory.Exists(TablePath);
        }

        public static List<string> GetColumnsList(string TableName, string DatabaseName)
        {
            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DatabaseName))
                return null;

            //if (!DoesTableExist(TableName, DatabaseName))
            //    return null;


            //temp
            return clsGlobal_DA.GetColumnsNamesList(TableName, DatabaseName);

        }

        public static DataTable GetEmptyTableStruct(string TableName, string DatabaseName)
        {
            List<string> Columns = GetColumnsList(TableName, DatabaseName);

            DataTable Table = new DataTable();

            foreach (string Column in Columns)
            {
                Table.Columns.Add(Column, typeof(string));
            }


            return Table;
        }

        public static string GetPrimaryKeyColumnName(string TableName, string DbName)
        {

            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DbName))
                return null;

            DataSet dataSet = clsXML.XmlToDataSet(clsPaths.GetTableStructureFilePath(DbName, TableName));

            for (int i = 0; i <  dataSet.Tables.Count; i++)
            {
                if (i  == 0) continue;

               foreach (DataRow dr in dataSet.Tables[i].Rows)
               {
                    try
                    {
                        if (Convert.ToBoolean(dr["PK"]))
                        {
                            return dataSet.Tables[i].TableName;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    
               }
            }


            return null;
        }

        public static List<clsColumnInfo> GetColumnsInfoList(string TableName, string DatabaseName)
        {
            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DatabaseName))
                return null;

            List<clsColumnInfo> ColumnsInfo = new List<clsColumnInfo>();

            DataSet set = clsXML.XmlToDataSet(clsPaths.GetTableStructureFilePath(DatabaseName, TableName));

            for (int i = 0; i < set.Tables.Count; i++)
            {
                //skipping the first dt
                if (i == 0) continue;

                ColumnsInfo.Add(_GetColumnInfo(set.Tables[i]));


            }


            return ColumnsInfo;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DatabaseName"></param>
        /// <param name="ColumnName"></param>
        /// <returns>
        /// the column info or NULL if the column does not exist in the table 
        /// </returns>
        public static clsColumnInfo GetColumnsInfo(string TableName, string DatabaseName, string ColumnName)
        {
            if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(DatabaseName) ||
                string.IsNullOrEmpty(ColumnName))
                return null;


            DataSet set = clsXML.XmlToDataSet(clsPaths.GetTableStructureFilePath(DatabaseName, TableName));

            for (int i = 0; i < set.Tables.Count; i++)
            {
                //skipping the first dt
                if (i == 0) continue;


                if (set.Tables[i].TableName == ColumnName)
                    return _GetColumnInfo(set.Tables[i]);


            }


            return null;


        }


        //






        //GLOBAL SELECT METHODS-------------------------------------------------------:


        /// <summary>
        /// this select approach is used in the clustered and not clustterd
        /// index search mode. it gets all columns and all the rows in the targeted table
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DataTable SelectTable(string DatabaseName, string TableName)
        {
            if (string.IsNullOrEmpty(TableName) ||
               string.IsNullOrEmpty(DatabaseName))
            {
                return null;
            }


            return _GetUnionedTargetTable(DatabaseName, TableName);
        }

        /// <summary>
        /// this select approach is used in the clustered and not clustterd
        /// index search mode. it gets the specified columns and all the rows if the targeted table
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DataTable SelectTable(string DatabaseName, string TableName, List<string> columnsNames)
        {
            if (string.IsNullOrEmpty(TableName) ||
               string.IsNullOrEmpty(DatabaseName))
            {
                return null;
            }


            return clsGlobal_DA.FilterColumns(
                _GetUnionedTargetTable(DatabaseName, TableName), columnsNames.ToArray());
        }


        //







        /// <summary>
        /// it validates the columns values and insert the row to the table.
        /// <para></para>
        /// this function does not validate if the columns exist in the table.
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="TableName"></param>
        /// <param name="ROW">
        ///  represents the row to insert
        /// <para></para>
        ///  the key represents the column info class
        ///  and the value is the column value.
        ///<para></para>
        ///  columns should exist in the table already.
        /// </param>
        public static void AddRow(string DatabaseName, string TableName, Dictionary<clsColumnInfo, object> ROW)
        {
         
            _ValidateRow(DatabaseName, TableName, ROW);


            long PkVal = clsGlobal_DA.GetPrimaryKeyValue(ref ROW);

            //PK = -1 means there is no PK
            if (_AddRowToPage(PkVal != -1, GetDataPagePathToInsert(
                PkVal, DatabaseName, TableName), TableName, DatabaseName, ROW))
            {
                _IncreaseRowCount(DatabaseName, TableName);
            }

        }





    }
}