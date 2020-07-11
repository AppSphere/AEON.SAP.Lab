using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AEON.SAP.Lab.XmlToDictionary.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Data;
using AEON.SAP.Converters;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace AEON.SAP.Lab.XmlToDictionary.Activities
{
    [LocalizedDisplayName(nameof(Resources.ProcessXMLForSAPBAPI_DisplayName))]
    [LocalizedDescription(nameof(Resources.ProcessXMLForSAPBAPI_Description))]
    public class ProcessXMLForSAPBAPI : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessXMLForSAPBAPI_PathToXML_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessXMLForSAPBAPI_PathToXML_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> PathToXML { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessXMLForSAPBAPI_XMLDocument_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessXMLForSAPBAPI_XMLDocument_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<XDocument> XMLDocument { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessXMLForSAPBAPI_Dictionary_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessXMLForSAPBAPI_Dictionary_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Dictionary<string, DataTable>> Dictionary { get; set; }

        #endregion


        #region Constructors

        public ProcessXMLForSAPBAPI()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var pathToXML = PathToXML.Get(context);
            var xmlDocument = XMLDocument.Get(context);
          
            SAPXmlConverter sapXmlConverter = new SAPXmlConverter();
            var dictionary = sapXmlConverter.ProcessXMLForSAPBapi(pathToXML, xmlDocument);

            // Outputs
            return (ctx) => {
                Dictionary.Set(ctx, dictionary);
            };
        }

        #endregion
    }
}

