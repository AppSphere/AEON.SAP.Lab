using AEON.SAP.Converters;
using System;

namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelToXMLConverter excelToXMLConverter = new ExcelToXMLConverter();
            var xmlDocument = excelToXMLConverter.ConvertExcelToXMLDocument("BAPI_SALESORDER_CREATEFROMDAT2.xlsx", "BAPI_SALESORDER_CREATEFROMDAT2");
            SAPXmlConverter sAPXmlConverter = new SAPXmlConverter();
            var output = sAPXmlConverter.ProcessXMLForSAPBapi("", xmlDocument);
        
        
        }
    }
}
