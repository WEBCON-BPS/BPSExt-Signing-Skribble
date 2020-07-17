using System;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Delete
{
    public class DeleteRequestAction : CustomAction<DeleteRequestActionConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            var log = new StringBuilder();

            try
            {
                var envelopeId = args.Context.CurrentDocument.GetFieldValue(Configuration.EnvelopeFielId).ToString();
                var skribble = new SkribbleHelper(log, Configuration.ApiConfig);
                if (Configuration.DeleteOperationType)
                    skribble.DeleteRequest(envelopeId);   
                else
                    skribble.DeclineSigRequest(envelopeId, Configuration.Message);
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