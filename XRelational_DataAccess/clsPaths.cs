using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace XRelational_DataAccess
{
    public static class clsPaths
    {


        public static string DatabasesPath = $@"C:\Program Files\xRelational\Databases";


        private static string _GetDataPageName(long PK, string DataPagesPath)
        {

            try
            {
                // Check if the folder exists
                if (!Directory.Exists(DataPagesPath))
                {
                    return "";
                }

                // Get all XML file paths within the folder
                string[] xmlFiles = Directory.GetFiles(DataPagesPath, "*.xml");

                // Loop through each XML file and read the file name
                foreach (string xmlFilePath in xmlFiles)
                {
                    string fileName = Path.GetFileName(xmlFilePath);
                    string[] Range = new string[2];
                    Range = fileName.Split('-');

                    //removing the '*.xml'
                    Range[1] = Range[1].Remove(Range[1].Length - 4, 4);

                    if (PK <= Convert.ToInt64(Range[1]) && PK >= Convert.ToInt64(Range[0]))
                        return $@"{Range[0]}-{Range[1]}";

                }
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        /// <summary>
        ///get a data page name out of the PK that might not even exist 
        /// </summary>
        /// <param name="PK"></param>
        /// <returns></returns>


        /// <summary>
        /// gets the specific page file path when it is searched by a clustered index
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="TabelName"></param>
        /// <param name="PK"></param>
        /// <returns></returns>
        public static string GetExistDataPagePath_CI(string DatabaseName, string TabelName, long PK)
        {
            string DataPagesPath =
                $@"{clsPaths.DatabasesPath}\{DatabaseName}\table_{TabelName}\DataPages\";

            string DataPagePath =  DataPagesPath  + _GetDataPageName(PK, DataPagesPath) + ".xml";

            if (!File.Exists(DataPagePath))
                return null;
            else
                return DataPagePath;
        }

        public static string GetTableDataPagesFolderPath(string DatabaseName, string TabelName)
        {
            return $@"{DatabasesPath}\{DatabaseName}\table_{TabelName}\DataPages";

        }

        public static string GetTableStructureFilePath(string DatabaseName, string TabelName)
        {
            return $@"{DatabasesPath}\{DatabaseName}\table_{TabelName}\TableStructure.xml";

        }

        public static string GetTableGlobeFilePath(string DatabaseName, string TabelName)
        {
            return $@"{DatabasesPath}\{DatabaseName}\table_{TabelName}\TableGlobe.xml";

        }

        public static string GetTableMainPage(string DatabaseName, string TabelName)
        {
            return $@"{DatabasesPath}\{DatabaseName}\table_{TabelName}\{clsConst.dataPage_MainPageName}.xml";

        }

        public static List<string> GetTableDataPagesPaths(string DatabaseName, string TabelName)
        {

            string DP_FolderPath = GetTableDataPagesFolderPath(DatabaseName, TabelName);

            // Check if the folder exists
            if (!Directory.Exists(DP_FolderPath))
                return null;


            string[] xmlFiles = Directory.GetFiles(DP_FolderPath, "*.xml");

            return xmlFiles.ToList();

        }

        public static string GetDataPagePath(string DatabaseName, string TabelName, string PageName)
        {
            return 
                $@"{clsPaths.DatabasesPath}\{DatabaseName}\table_{TabelName}\DataPages\{PageName}.xml";
        }

        public static string GetDataPagePath_CI(string DatabaseName, string TabelName, long PK)
        {
            string DataPagesPath =
                $@"{clsPaths.DatabasesPath}\{DatabaseName}\table_{TabelName}\DataPages\";

            string DataPagePath = DataPagesPath + GetDataPageName(PK) + ".xml";


            return DataPagePath;    
        }

        public static string GetDataPageName(long PK)
        {
            if (PK <= 0) return null;

            int DivCount = (int)(PK / clsConst.dataPageRowsCapacity);
     
            return $@"{DivCount * 100 + 1}-{DivCount * 100 + 100}";
        }



    }
}
