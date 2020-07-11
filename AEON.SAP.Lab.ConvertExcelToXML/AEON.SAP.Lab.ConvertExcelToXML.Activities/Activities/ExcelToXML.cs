using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AEON.SAP.Converters;
using AEON.SAP.Lab.ConvertExcelToXML.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace AEON.SAP.Lab.ConvertExcelToXML.Activities
{
    [LocalizedDisplayName(nameof(Resources.ExcelToXML_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExcelToXML_Description))]
    public class ExcelToXML : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXML_ExcelFilePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXML_ExcelFilePath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ExcelFilePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXML_SAPProcessName_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXML_SAPProcessName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> SAPProcessName { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXML_XMLFolderPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXML_XMLFolderPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> XMLFolderPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXML_XMLDocument_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXML_XMLDocument_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<XmlDocument> XMLDocument { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXML_XMLPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXML_XMLPath_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> XMLPath { get; set; }

        #endregion


        #region Constructors

        public ExcelToXML()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ExcelFilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ExcelFilePath)));
            if (SAPProcessName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(SAPProcessName)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var excelFilePath = ExcelFilePath.Get(context);
            var sapProcessName = SAPProcessName.Get(context);
            var xmlFolderPath = XMLFolderPath.Get(context);


            var xmlFilePath = string.Empty;
            XmlDocument doc = null;
      
            ExcelToXMLConverter excelToXMLConverter = new ExcelToXMLConverter();
            if(!string.IsNullOrEmpty(xmlFolderPath))
            {
                xmlFilePath = excelToXMLConverter.ConvertExcelToXMLFile(excelFilePath, sapProcessName, xmlFolderPath);
            }
            else
            {
                doc = excelToXMLConverter.ConvertExcelToXMLDocument(excelFilePath, sapProcessName);
            }

            // Outputs
            return (ctx) => {
                XMLDocument.Set(ctx, doc);
                XMLPath.Set(ctx, xmlFilePath);
            };
        }

        #endregion
    }
}

