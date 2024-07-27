using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XRelational_DataAccess
{
    public static class clsXML
    {
      


        /// <summary>
        /// this functions gets a table from the entered XML file 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="tableIndex">
        /// 0 - gets a datatable of the root attributes
        /// 1- gets a datatabel of the elements
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable XmlToDataTable(string xmlFilePath, byte tableIndex)
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet.ReadXml(xmlFilePath);
                return dataSet.Tables[tableIndex];
            }
            catch
            {
                return null;
            }

        }

        public static DataSet XmlToDataSet(string xmlFilePath)
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet.ReadXml(xmlFilePath);
                return dataSet;
            }
            catch
            {
                return null;
            }

        }


        public static bool CreateXmlFile(string path, string fileName, string RootName)
        {
            // Combine the path and file name to create the full file path
            string filePath = Path.Combine(path, fileName + ".xml");

            // Check if the directory exists, and create it if it doesn't
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Create the XML document
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML declaration
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);

            // Create the root element
            XmlElement rootElement = xmlDocument.CreateElement(RootName);
            xmlDocument.AppendChild(rootElement);


            try
            {
                // Save the XML document to the file
                xmlDocument.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }


        }


        public static bool AddXmlElementToRoot(string elementName,
          string elementValue, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                // Load the existing XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                // Create the new element
                XmlElement newElement = xmlDoc.CreateElement(elementName);
                newElement.InnerText = elementValue;

                // Get the root element and append the new element to it
                XmlElement rootElement = xmlDoc.DocumentElement;
                rootElement.AppendChild(newElement);

                // Save the changes to the XML document
                xmlDoc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        public static bool AddXmlElement(string ParentElement,int ParentElementIndex, string elementName,
            string elementValue, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                // Load the existing XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                // Create the new element
                XmlElement newElement = xmlDoc.CreateElement(elementName);
                newElement.InnerText = elementValue;

                // Get the root element and append the new element to it
                XmlNodeList rootElement = xmlDoc.GetElementsByTagName(ParentElement);
                rootElement[ParentElementIndex].AppendChild(newElement);
                
                // Save the changes to the XML document
                xmlDoc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }



        public static bool AddXmlsubElementToLastElement(string ParentElement, string elementName,
          string elementValue, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                // Load the existing XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                // Create the new element
                XmlElement newElement = xmlDoc.CreateElement(elementName);
                newElement.InnerText = elementValue;

                // Get the root element and append the new element to it
                XmlNodeList rootElement = xmlDoc.GetElementsByTagName(ParentElement);
                rootElement[rootElement.Count -1].AppendChild(newElement);

                // Save the changes to the XML document
                xmlDoc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }





        public static bool AddXmlAttribute(string AttributeName, string AttributeValue, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                // Load the existing XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                // Create the new element
                XmlAttribute newAttribute = xmlDoc.CreateAttribute(AttributeName);
                newAttribute.InnerText = AttributeValue;

                // Get the root element and append the new element to it
                XmlElement rootElement = xmlDoc.DocumentElement;
                rootElement.Attributes.Append(newAttribute);

                // Save the changes to the XML document
                xmlDoc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public static bool RenameXmlFile(string filePath, string newName)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(filePath);
            string newFilePath = Path.Combine(directory, newName);

            try
            {
                File.Move(filePath, newFilePath);
                return true;
            }
            catch (IOException e)
            {
                return true;        
            
            }

        }


        public static bool InsertXmlElementAfter(string xmlFilePath,
            string previousElementName, string newElementName, string newElementValue)
        {

            if (!File.Exists(xmlFilePath))
            {
                return false;
            }

            // Load the XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            // Find the previous element
            XmlNode previousElement = xmlDoc.SelectSingleNode($"//{previousElementName}");

            if (previousElement != null)
            {
                // Create the new element
                XmlElement newElement = xmlDoc.CreateElement(newElementName);
                newElement.InnerText = newElementValue;

                // Insert the new element after the previous element
                previousElement.ParentNode.InsertAfter(newElement, previousElement);
            }
            else
            {
                return false;
            }

            // Save the modified XML document
            xmlDoc.Save(xmlFilePath);
            return true;
        }

        public static string GetLastElementParentName(string xmlFilePath)
        {
            XDocument xmlDocument = XDocument.Load(xmlFilePath);

            XElement lastElement = xmlDocument.Descendants().Last().Parent;

            return lastElement?.Name.LocalName;
        }


        public static void ClearXmlFile(string filePath)
        {
            // Load the XML document
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Get the root element
            var rootElement = xmlDoc.DocumentElement;

            // Remove all child elements of the root while preserving attributes
            while (rootElement.HasChildNodes)
            {
                rootElement.RemoveChild(rootElement.FirstChild);
            }

            // Save the modified XML back to the file
            xmlDoc.Save(filePath);
        }



        public static void InsertDataTableToXml(ref DataTable dataTable, string xmlFilePath,
            List<string> TableColumnsToAdd)
        {
            // Create a new XML document
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(xmlFilePath);

            XmlNode rootElement = xmlDoc.DocumentElement;

            // Iterate over the rows in the DataTable
            foreach (DataRow row in dataTable.Rows)
            {

               
                // Create a child element for each row
                var rowElement = xmlDoc.CreateElement(clsConst.xml_DataPageElementName);
                rootElement.AppendChild(rowElement);

                // Iterate over the columns in the row
                foreach (DataColumn column in dataTable.Columns)
                {
                   //skipping the non related columns to the table 
                    if (!TableColumnsToAdd.Contains(column.ColumnName))
                        continue;


                    // Get the column name and value
                    var columnName = column.ColumnName;
                    var columnValue = row[columnName]?.ToString() ?? string.Empty;

                    // Create a child element for each column
                    var columnElement = xmlDoc.CreateElement(columnName);
                    columnElement.InnerText = columnValue;
                    rowElement.AppendChild(columnElement);
                }
            }

            // Save the XML document to the file
            xmlDoc.Save(xmlFilePath);
        }


        public static string GetElementValue(string xmlFilePath, string elementName)
        {
            string elementValue = null;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName(elementName);

                if (nodeList.Count > 0)
                {
                    XmlNode node = nodeList[0];
                    elementValue = node.InnerText;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            return elementValue;
        }


        public static bool UpdateElementValue(string xmlFilePath, string elementName, string newValue)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName(elementName);

                if (nodeList.Count > 0)
                {
                    XmlNode node = nodeList[0];
                    node.InnerText = newValue;

                    xmlDoc.Save(xmlFilePath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle any exception that occurred while parsing the XML
                return false;
            }

            return false;
        }
    }
}
