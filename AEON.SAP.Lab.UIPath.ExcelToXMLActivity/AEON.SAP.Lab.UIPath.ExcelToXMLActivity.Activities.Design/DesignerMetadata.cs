using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using AEON.SAP.Lab.UIPath.ExcelToXMLActivity.Activities.Design.Designers;
using AEON.SAP.Lab.UIPath.ExcelToXMLActivity.Activities.Design.Properties;

namespace AEON.SAP.Lab.UIPath.ExcelToXMLActivity.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ExcelToXMLActivity), categoryAttribute);
            builder.AddCustomAttributes(typeof(ExcelToXMLActivity), new DesignerAttribute(typeof(ExcelToXMLActivityDesigner)));
            builder.AddCustomAttributes(typeof(ExcelToXMLActivity), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
