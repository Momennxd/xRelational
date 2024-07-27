using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRelational_DataAccess;

namespace XRelational_Engine
{
    public class clsTableGlob
    {
      
        /// <summary>
        /// represents the number of the rows in the assossiated table
        /// </summary>
        public long RowsCount { get; private set; }

        public string TableName { get; private set; }

        public string DatabaseName { get; private set; }



        public static clsTableGlob Find(string TableName, string DatabaseName)
        {
            DataSet set = clsXML.XmlToDataSet(clsPaths.GetTableGlobeFilePath(DatabaseName, TableName));

            clsTableGlob tableGlob = new clsTableGlob();
           
            foreach (DataTable dt in set.Tables)
            {
                if (dt.TableName == "TableInfo")
                {
                    tableGlob.RowsCount = Convert.ToInt64(dt.Rows[0]["RowsCount"]);

                }
            }


            tableGlob.DatabaseName = DatabaseName;
            tableGlob.TableName = TableName;

            return tableGlob;
        }

        public static long GetRowsCount(string TableName, string DatabaseName)
        {
            DataSet set = clsXML.XmlToDataSet(clsPaths.GetTableGlobeFilePath(DatabaseName, TableName));

            foreach (DataTable dt in set.Tables)
            {
                if (dt.TableName == "TableInfo")
                {
                    return Convert.ToInt64(dt.Rows[0][clsConst.RowsCountNameInTablGlobe]);
                }
            }


            return 0;
        }



    }
}
