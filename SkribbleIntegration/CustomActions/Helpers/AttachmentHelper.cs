using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers
{
    internal sealed class AttachmentHelper
    {
        internal static AttachmentData GetAttachment(ActionContextInfo context, InputAttConfig attConfig, StringBuilder log)
        {
            if (attConfig.InputAttType == InputType.Category)
            {
                log.AppendLine("Downloading attachments by category");

                var allAttachments = DocumentAttachmentsManager.GetAttachments(new GetAttachmentsParams()
                {
                    DocumentId = context.CurrentDocument.ID,
                    IncludeContent = true
                });

                if (attConfig.CatType == CategoryType.ID)
                {
                    return allAttachments.FirstOrDefault(x =>
                    x.FileGroup.ID == attConfig.InputCategoryId.ToString()
                    && (string.IsNullOrEmpty(attConfig.AttRegularExpression) || Regex.IsMatch(x.FileName, attConfig.AttRegularExpression)));
                }
                else if (attConfig.CatType == CategoryType.None)
                {
                    return allAttachments.FirstOrDefault(x =>
                    x.FileGroup == null
                    && (string.IsNullOrEmpty(attConfig.AttRegularExpression) || Regex.IsMatch(x.FileName, attConfig.AttRegularExpression)));
                }
                else
                {
                    return allAttachments.First();
                }
            }
            else
            {
                log.AppendLine("Downloading attachments by SQL query");

                var attId = SqlExecutionHelper.ExecSqlCommandScalar(attConfig.AttQuery, context);
                if (attId == null)
                    throw new Exception("Sql query not returning result");

                return DocumentAttachmentsManager.GetAttachment(Convert.ToInt32(attId));
            }
        }
    }
}
