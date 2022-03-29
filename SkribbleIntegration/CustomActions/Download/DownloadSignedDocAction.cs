using System;
using System.IO;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Download
{
    public class DownloadSignedDocAction : CustomAction<DownloadSignedDocActionConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            var log = new StringBuilder();

            try
            {
                var status = args.Context.CurrentDocument.GetFieldValue(Configuration.StatusFielId)?.ToString();
                if (!string.IsNullOrEmpty(status) && status.Equals(Models.Statuses.Signed))
                {
                    var docGuid = args.Context.CurrentDocument.GetFieldValue(Configuration.GuidIdFielId).ToString();
                    var response = new SkribbleHelper(log, Configuration.ApiConfig).GetDocumentContent(docGuid);
                    SaveAtt(args.Context, response);
                }
                else
                {
                    args.HasErrors = true;
                    args.Message = "Document cannot be a saved if it is not signed";
                }
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

        private void SaveAtt(ActionContextInfo context, byte[] newAttContent)
        {
            if (newAttContent == null)
                throw new Exception("API returned empty document content!");

            int sourceAttData = Convert.ToInt32(context.CurrentDocument.GetFieldValue(Configuration.AttConfig.AttTechnicalFieldID));            
            
            var sourceAtt = context.CurrentDocument.Attachments.GetByID(sourceAttData);         
            sourceAtt.Content = newAttContent;
            sourceAtt.FileName = $"{Path.GetFileNameWithoutExtension(sourceAtt.FileName)}{Configuration.AttConfig.AttSufix}{sourceAtt.FileExtension}";

            if (!string.IsNullOrEmpty(Configuration.AttConfig.SaveCategory))
            {
                sourceAtt.FileGroup = new WorkFlow.SDK.Documents.Model.Attachments.AttachmentsGroup(Configuration.AttConfig.SaveCategory, null);
            }

            new DocumentAttachmentsManager(context).UpdateAttachment(new WorkFlow.SDK.Documents.Model.Attachments.UpdateAttachmentParams()
            {
                Attachment = sourceAtt
            });
        }
    }
}