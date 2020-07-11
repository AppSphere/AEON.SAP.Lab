using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Aeon.SAP.Lab.ConvertXMLToDict.Activities.Design.Designers;
using Aeon.SAP.Lab.ConvertXMLToDict.Activities.Design.Properties;

namespace Aeon.SAP.Lab.ConvertXMLToDict.Activities.Design
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
