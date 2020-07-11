using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AEON.SAP.Converters
{
    /*
       The main Math class
       Contains all methods for performing basic math functions
   */
    /// <summary>
    /// The main <c>SAPXmlConverter</c> class.
    /// Contains all methods for handling XML to SAP Compatible Dictionary of Data Table
    /// </summary>
    public class SAPXmlConverter
    {
        /// <summary>
        /// This mehod Return the SAP Bapi Compatible Data Table Colelction in a Dictionary
        /// </summary>
        /// <param name="xmlFilePath">XML File Path</param>
        /// <param name="xmlDocument">XMl Document</param>
        /// <returns>Dictionay of DataTable</returns>
        public Dictionary<string, DataTable> ProcessXMLForSAPBapi(string xmlFilePath = "", XmlDocument xmlDoc = null)
        {
            var output = new Dictionary<string, DataTable>();
            try
            {
                if (xmlDoc != null)
                {
                    var xmlDocument = xmlDoc.ToXDocument();
                    return this.ProcessXmlToDictionaryUsingXDocument(xmlDocument, output);
                }
                else if (!string.IsNullOrEmpty(xmlFilePath))
                {
                    return this.ProcessXmlToDictionaryUsingXmlFilePath(xmlFilePath, output);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;

        }

        public XDocument GetXDocument(XmlDocument doc)
        {
            return doc.ToXDocument();
        }

        /// <summary>
        /// This method withh return the Dictionary of DataTables based on the XML File Path
        /// </summary>
        /// <param name="xmlFilePath">XML File Path</param>
        /// <param name="output">Dictionary of DataTable</param>
        /// <returns>Dictionary</returns>
        private Dictionary<string, DataTable> ProcessXmlToDictionaryUsingXmlFilePath(string xmlFilePath, Dictionary<string, DataTable> output)
        {
            XDocument xDoc = XDocument.Load(xmlFilePath);

            if (xDoc != null)
            {
                XElement root = xDoc.Root;
                DataTable dataTable = new DataTable(root.Name.ToString());

                var DT_BAPI_Parameters = ParseNonDataRowDataTableNodes(output, root);
                output.Add("DT_BAPI_Parameters", DT_BAPI_Parameters);
                ParseDataTableNodes(output, root, "DataTable", "DataTable/*");
                ParseDataTableNodes(output, root, "DataRow", "row");

                // Outputs
                return output;
            }

            return null;
        }

        /// <summary>
        /// This method will returns the Dictionary based on the XDocument Passed
        /// </summary>
        /// <param name="xDocument">xml Document</param>
        /// <param name="output">Dictionary</param>
        /// <returns>Dictionary Of DataTables</returns>
        private Dictionary<string, DataTable> ProcessXmlToDictionaryUsingXDocument(XDocument xDocument, Dictionary<string, DataTable> output)
        {
            if (xDocument != null)
            {
                XElement root = xDocument.Root;
                DataTable dataTable = new DataTable(root.Name.ToString());

                var DT_BAPI_Parameters = ParseNonDataRowDataTableNodes(output, root);
                output.Add("DT_BAPI_Parameters", DT_BAPI_Parameters);
                ParseDataTableNodes(output, root, "DataTable", "DataTable/*");
                ParseDataTableNodes(output, root, "DataRow", "row");

                // Outputs
                return output;
            }

            return null;
        }

        /// <summary>
        /// This method will return the DataTable for Nodes marked as DataTable or DataRow
        /// </summary>
        /// <param name="output">Dictionary</param>
        /// <param name="root">XML Root</param>
        /// <param name="nodeType">Node Type</param>
        /// <param name="xPath">Path to Search</param>
        /// <returns>Dictionary</returns>
        private Dictionary<string, DataTable> ParseDataTableNodes(Dictionary<string, DataTable> output, XElement root, string nodeType = "DataTable", string xPath = "DataTable/*")
        {

            // Now get the All the Elements of this XML where the Data Type is not Data Table or Data ROw
            var xNodes = root.Elements().Where(x => x.Attribute("DataType").Value == nodeType);

            foreach (var xNode in xNodes)
            {
                DataTable DT_TABLES = new DataTable(string.Format("{0}_{1}", "DT_", xNode.Name.ToString()));
                var element = xNode.XPathSelectElements(xPath);
                // check if node has Chile Nodes
                if (element.Descendants().Count() > 0)
                {
                    var returnTable = GenerateDataTable(DT_TABLES, element);
                    output.Add(string.Format("{0}_{1}", "DT_", xNode.Name.ToString()), returnTable);
                }
            }

            return output;

        }

        /// <summary>
        /// This method returns the Data Table for all the nodes not marked as "DataTable" or "DataRow"
        /// </summary>
        /// <param name="output">Dictionary of DataTables</param>
        /// <param name="root">root node</param>
        /// <returns>DataTable</returns>
        private DataTable ParseNonDataRowDataTableNodes(Dictionary<string, DataTable> output, XElement root)
        {
            // This table will be a home of all the Non Data Table and Data Row Types
            DataTable DT_BAPI_Parameters = new DataTable("DT_BAPI_Parameters");

            // Now get the All the Elements of this XML where the Data Type is not Data Table or Data ROw
            var xNodes = root.Elements().Where(x => x.Attribute("DataType").Value != "DataTable" && x.Attribute("DataType").Value != "DataRow");

            DT_BAPI_Parameters.Columns.AddRange(new DataColumn[xNodes.Count()]);
            return GenerateDataTable(DT_BAPI_Parameters, xNodes);

        }

        /// <summary>
        /// This Method Generates the DataTable based on Collection of Nodes
        /// </summary>
        /// <param name="dataTable">DatTable</param>
        /// <param name="xNodes">XML Node Collections</param>
        /// <returns>DataTable</returns>
        private DataTable GenerateDataTable(DataTable dataTable, IEnumerable<XElement> xNodes)
        {
            foreach (var xNode in xNodes)
            {
                if (xNode.Name.ToString().Equals("row"))
                {
                    var childRows = xNode.Elements();
                    foreach (var childRow in childRows)
                    {
                        CreateDataTableColumns(dataTable, childRow);
                    }
                }
                else
                {
                    CreateDataTableColumns(dataTable, xNode);
                }

            }

            DataRow row;
            bool isTableRowCase;
            SetDataRowInDataTableCollection(dataTable, xNodes, out row, out isTableRowCase);
            if (!isTableRowCase)
                dataTable.Rows.Add(row);

            return dataTable;
        }

        /// <summary>
        /// This method will create the create the data table based on the XML Nodes
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="xNode">XML Node</param>
        private void CreateDataTableColumns(DataTable dataTable, XElement xNode)
        {
            if (!dataTable.Columns.Contains(xNode.Name.ToString()))
            {
                var column = new DataColumn(xNode.Name.ToString());
                GetNodeType(xNode, column);
                GetNodeDescription(xNode, column);
                GetNodeLength(xNode, column);
                dataTable.Columns.Add(column);
            }
        }

        /// <summary>
        /// This method will set the row in datatable colection
        /// </summary>
        /// <param name="dataTable">Data Table</param>
        /// <param name="xNodes">Collection of Nodes</param>
        /// <param name="row">Data Row</param>
        /// <param name="isTableRowCase">Is this is a Row inside a Table</param>
        private void SetDataRowInDataTableCollection(DataTable dataTable, IEnumerable<XElement> xNodes, out DataRow row, out bool isTableRowCase)
        {
            // Now Lets Fill this Data table
            row = dataTable.NewRow();
            isTableRowCase = false;
            foreach (var xNode in xNodes)
            {
                if (xNode.Name.ToString().Equals("row"))
                {
                    var innerRow = dataTable.NewRow();
                    isTableRowCase = true;
                    var childOfRowNodes = xNode.Elements();
                    foreach (var childRow in childOfRowNodes)
                    {
                        FillRow(innerRow, childRow);
                    }
                    dataTable.Rows.Add(innerRow);
                }
                else
                {
                    string dataValue = GetNodeValue(xNode);
                    if (dataValue != null)
                    {
                        FillRow(row, xNode);
                    }
                }

            }
        }

        /// <summary>
        /// This method will Fill the DataRow with a Value Passed
        /// </summary>
        /// <param name="innerRow">DataROw</param>
        /// <param name="childRow">Child Node</param>
        private void FillRow(DataRow innerRow, XElement childRow)
        {
            string dataValue = GetNodeValue(childRow);
            if (dataValue != null)
            {
                var dataType = childRow.Attribute("DataType").Value;
                var type = GetNodeTypeOf(dataType);
                innerRow[childRow.Name.ToString()] = Convert.ChangeType(dataValue, type);
            }
        }

        /// <summary>
        /// This method returns the Values of a Node, Node can contain Value in an Attribute or in a element
        /// </summary>
        /// <param name="xNode">XML Node</param>
        /// <returns>Value</returns>
        private string GetNodeValue(XElement xNode)
        {
            if (xNode.Attribute("value") != null)
            {
                string dataValue = xNode.Attribute("value").Value;
                if (string.IsNullOrEmpty(dataValue))
                {
                    // Check if the Main Element contains this
                    if (!string.IsNullOrEmpty(xNode.Value))
                    {
                        dataValue = xNode.Value;
                        return dataValue;

                    }
                }
                else
                {
                    return dataValue;
                }
            }

            return null;

        }

        /// <summary>
        /// This method will return the Description of the Node
        /// </summary>
        /// <param name="xNode">XML Node</param>
        /// <param name="column">Cpation of the Column will be set</param>
        private void GetNodeDescription(XElement xNode, DataColumn column)
        {
            // Assign Column Description
            column.Caption = xNode.Attribute("Description").Value;
        }

        /// <summary>
        /// This method will set the length of the column
        /// </summary>
        /// <param name="xNode">Xml Node</param>
        /// <param name="column">Table Column</param>
        private void GetNodeLength(XElement xNode, DataColumn column)
        {
            // Check the Length
            string xlength = xNode.Attribute("Length").Value;
            int length = 0;
            if (!string.IsNullOrEmpty(xlength))
            {
                length = Int32.Parse(xlength);
                if (length > 1)
                {
                    column.MaxLength = length;

                }
            }
        }

        /// <summary>
        /// This method will set the Type based on the "type" Attribute of the node
        /// </summary>
        /// <param name="xNode">XML Node</param>
        /// <param name="column"></param>
        private void GetNodeType(XElement xNode, DataColumn column)
        {
            string dataType = xNode.Attribute("DataType").Value;
            var type = GetNodeTypeOf(dataType);
            column.DataType = type;
        }

        /// <summary>
        /// This Nodes returns the DataType based on the datatype passed
        /// </summary>
        /// <param name="dataType">Data Type in string</param>
        /// <returns>Return the Type</returns>
        private Type GetNodeTypeOf(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "int": return typeof(Int32);
                case "string": return typeof(string);

            }

            return typeof(string);
        }

    }
}
