using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using AEON.SAP.Lab.ConvertExcelToXML.Activities.Design.Designers;
using AEON.SAP.Lab.ConvertExcelToXML.Activities.Design.Properties;

namespace AEON.SAP.Lab.ConvertExcelToXML.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ExcelToXML), categoryAttribute);
            builder.AddCustomAttributes(typeof(ExcelToXML), new DesignerAttribute(typeof(ExcelToXMLDesigner)));
            builder.AddCustomAttributes(typeof(ExcelToXML), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
