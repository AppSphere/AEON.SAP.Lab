using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AEON.SAP.Converters
{
    public class ExcelToXMLConverter
    {
        public static List<string> attributes = new List<string>();
      


        /// <summary>
        /// This method Returns the XML File Based on the SAP Excel Supplied.
        /// </summary>
        /// <param name="excelFilePath">Path to the Excel File</param>
        /// <param name="processName">SAP Process Name</param>
        /// <param name="xmlPath">Return XML Path</param>
        /// <returns>Return XML Path</returns>
        public string ConvertExcelToXMLFile(string excelFilePath, string processName, string xmlPath = "")
        {
            var xmlFile = GenerateXmlFile(excelFilePath, processName, xmlPath);
            return xmlFile;
        }

        /// <summary>
        /// This method Returns the XML File Based on the SAP Excel Supplied.
        /// </summary>
        /// <param name="excelFilePath">Path to the Excel File</param>
        /// <param name="processName">SAP Process Name</param>
        /// <param name="xmlPath">Return XML Path</param>
        /// <returns>Return XML Document</returns>
        public XmlDocument ConvertExcelToXMLDocument(string excelFilePath, string processName, string xmlPath = "")
        {
            var xmlFile = GenerateXmlFile(excelFilePath, processName, xmlPath);
            XmlDocument xmlDocument = new XmlDocument();
            // Read File into a String
            var xmlContent = File.ReadAllText(xmlFile);
            xmlDocument.LoadXml(xmlContent);
            return xmlDocument;
        }

        private string GenerateXmlFile(string filePath, string processName, string xmlPath = "")
        {
            var mainTableWithRules = ImportExceltoDatatable(filePath, processName);
            ////collect all the Attributes
            for (int rowCount = 1; rowCount < mainTableWithRules.Rows.Count; rowCount++)
            {
                attributes.Add(mainTableWithRules.Rows[rowCount][0].ToString());
            }

            string xmlFileName = processName + ".XML";
            if (!string.IsNullOrEmpty(xmlPath))
            {
                xmlFileName = xmlPath + "/" + xmlFileName;
            }
            using (XmlWriter writer = XmlWriter.Create(xmlFileName))
            {
                writer.WriteStartDocument();
                CreateXMLNodes(writer, mainTableWithRules, filePath, processName);
                writer.WriteEndDocument();
            }

            return xmlFileName;
        }

        public void CreateXMLNodes(XmlWriter writer, DataTable mainTableWithRules,string filePath, string sheetName, bool isChildren = false)
        {

            if (!isChildren)
                writer.WriteStartElement(sheetName);

            //print all the columns as a Tag
            var tagRow = mainTableWithRules.Rows[0]; // Take the First Row

            for (int mainRowCol = 1; mainRowCol < tagRow.ItemArray.Length; mainRowCol++)
            {
                bool containsChildren = false;
                bool isContainValue = false;
                List<string> values = new List<string>();
                for (int i = 1; i < mainTableWithRules.Rows.Count; i++)
                {
                    if (!isContainValue && mainTableWithRules.Rows[i][mainRowCol].ToString().Contains("DataRow") || mainTableWithRules.Rows[i][mainRowCol].ToString().Contains("DataRow"))
                    {
                        isContainValue = true;
                    }

                    values.Add(mainTableWithRules.Rows[i][mainRowCol].ToString());

                }

                writer.WriteStartElement(tagRow[mainRowCol].ToString());

                if (!values.Contains("DataRow") || !values.Contains("DataTable"))
                {
                    //write all the Attributes
                    for (int attrCount = 0; attrCount < attributes.Count; attrCount++)
                    {

                        writer.WriteAttributeString(attributes[attrCount].ToString(), values[attrCount] != null ? values[attrCount].ToString() : string.Empty);
                        if (!containsChildren && values[attrCount].ToString().Equals("DataRow") || values[attrCount].ToString().Equals("DataTable"))
                        {
                            containsChildren = true;
                        }

                    }

                }

                if (!isChildren && (!values.Contains("DataRow") && !values.Contains("DataTable")))
                {
                    writer.WriteAttributeString("value", string.Empty);
                }

                if (isChildren)
                    writer.WriteAttributeString("value", string.Empty);



                if (values.Contains("DataRow"))
                {
                    writer.WriteStartElement("row");
                }

                if (values.Contains("DataTable"))
                {
                    writer.WriteStartElement("DataTable");
                    writer.WriteStartElement("row");

                }




                if (containsChildren)
                {
                    // Now check the Sheets if this Exists
                    var children = ImportExceltoDatatable(filePath, tagRow[mainRowCol].ToString());
                    CreateXMLNodes(writer, children, tagRow[mainRowCol].ToString(),filePath, true);
                    containsChildren = false;

                }

                writer.WriteEndElement();
                if (values.Contains("DataRow"))
                {
                    writer.WriteEndElement();
                }

                if (values.Contains("DataTable"))
                {
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

            }
            if (!isChildren)
                writer.WriteEndElement();
        }

        public DataTable ImportExceltoDatatable(string filePath, string sheetName)
        {
            // Open the Excel file using ClosedXML.
            // Keep in mind the Excel file cannot be open when trying to read it
   
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(sheetName);

                //Create a new DataTable.
                DataTable dt = new DataTable();

                var headerRow = workSheet.FirstRowUsed();

                foreach (IXLCell cell in headerRow.Cells())
                {
                    dt.Columns.Add(cell.Value.ToString());
                }
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Add rows to DataTable.
                    dt.Rows.Add();
                    int i = 0;

                    foreach (IXLCell cell in row.Cells())
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                        i++;
                    }

                }

                return dt;
            }
        }


    }
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
