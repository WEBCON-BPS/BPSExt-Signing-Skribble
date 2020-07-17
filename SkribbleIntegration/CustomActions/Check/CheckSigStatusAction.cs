using System;
using System.Text;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Check
{
    public class CheckSigStatusAction : CustomAction<CheckSigStatusActionConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            var log = new StringBuilder();

            try
            {              
                var envelopeId = args.Context.CurrentDocument.GetFieldValue(Configuration.RequestParams.EnvelopeIdFielId)?.ToString();
                var response = new SkribbleHelper(log, Configuration.ApiConfig).CheckDocumentStatus(envelopeId);               

                if(response.status_overall.Equals(Models.Statuses.Signed))
                      args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.GuidFielId, response.document_id);

                args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.StatusFieldId, response.status_overall);
            }
            catch (Exception e)
            {
                log.AppendLine(e.ToString());
                args.HasErrors = true;
                args.Message = e.Message;
            }
            finally
            {
                args.LogMessage = log.ToString();
                args.Context.PluginLogger.AppendInfo(log.ToString());
            }
        }
    }
}