using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AEON.SAP.Lab.UIPath.ExcelToXMLActivity.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace AEON.SAP.Lab.UIPath.ExcelToXMLActivity.Activities
{
    [LocalizedDisplayName(nameof(Resources.ExcelToXMLActivity_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExcelToXMLActivity_Description))]
    public class ExcelToXMLActivity : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXMLActivity_ExcelFile_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXMLActivity_ExcelFile_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ExcelFile { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXMLActivity_XMLDocument_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXMLActivity_XMLDocument_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<XmlDocument> XMLDocument { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXMLActivity_XMLFIlePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXMLActivity_XMLFIlePath_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> XMLFIlePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExcelToXMLActivity_XMLFilePathFolder_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExcelToXMLActivity_XMLFilePathFolder_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> XMLFilePathFolder { get; set; }

        #endregion


        #region Constructors

        public ExcelToXMLActivity()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ExcelFile == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ExcelFile)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var excelfile = ExcelFile.Get(context);
            var xmlFilePathFolder = XMLFilePathFolder.Get(context);

            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
            ExcelToXMLConverter 


            // Outputs
            return (ctx) => {
                XMLDocument.Set(ctx, null);
                XMLFIlePath.Set(ctx, null);
            };
        }

        #endregion
    }
}

