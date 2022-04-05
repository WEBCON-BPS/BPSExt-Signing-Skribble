using System;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models;
using System.Collections.Generic;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.WorkFlow.SDK.Tools.Other;
using System.Linq;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Reminder
{
    public class SendReminderAction : CustomAction<SendReminderActionConfig>
    {
        StringBuilder _log = new StringBuilder();

        public override void Run(RunCustomActionParams args)
        {
            try
            {
                var status = args.Context.CurrentDocument.GetFieldValue(Configuration.StatusFielId)?.ToString();
                if (!string.IsNullOrEmpty(status) && status.Equals(Statuses.Open))
                {
                    var userList = args.Context.CurrentDocument.ItemsLists.GetByID(Configuration.Users.SignersList.ItemListId);
                    var guid = args.Context.CurrentDocument.GetFieldValue(Configuration.EnvelopeFielId)?.ToString();
                    var skribble = new SkribbleHelper(_log, Configuration.ApiConfig);
                    var newDocId = skribble.SendReminder(guid, Configuration.Message, PrepareUsers(userList));
                    args.Context.CurrentDocument.SetFieldValue(Configuration.GuidFielId, newDocId);
                }
            }
            catch (Exception e)
            {
                _log.AppendLine(e.ToString());
                args.HasErrors = true;
                args.Message = e.Message;
            }
            finally
            {
                args.LogMessage = _log.ToString();
                args.Context.PluginLogger.AppendInfo(_log.ToString());
            }
        }

        private List<RequestSignature> PrepareUsers(ItemsList itemsList)
        {
            if (itemsList.Rows.Count <= 0)
                throw new Exception("Empty signers list");

            var users = new List<RequestSignature>();
            foreach (var row in itemsList.Rows)
            {
                var user = new RequestSignature();
                user.signer_email_address = row.GetCellValue(Configuration.Users.SignersList.SignerMailColumnID).ToString();
                user.signer_identity_data = new SignerIdentityData()
                {
                    first_name = TextHelper.GetPairName(row.GetCellValue(Configuration.Users.SignersList.SignerNameColumnID).ToString()).Split(' ').First(),
                    last_name = TextHelper.GetPairName(row.GetCellValue(Configuration.Users.SignersList.SignerNameColumnID).ToString()).Split(' ').Last(),
                };

                users.Add(user);
            }

            return users;
        }
    }
}