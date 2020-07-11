using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using AEON.SAP.Lab.XmlToDictionary.Activities.Design.Designers;
using AEON.SAP.Lab.XmlToDictionary.Activities.Design.Properties;

namespace AEON.SAP.Lab.XmlToDictionary.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ProcessXMLForSAPBAPI), categoryAttribute);
            builder.AddCustomAttributes(typeof(ProcessXMLForSAPBAPI), new DesignerAttribute(typeof(ProcessXMLForSAPBAPIDesigner)));
            builder.AddCustomAttributes(typeof(ProcessXMLForSAPBAPI), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
