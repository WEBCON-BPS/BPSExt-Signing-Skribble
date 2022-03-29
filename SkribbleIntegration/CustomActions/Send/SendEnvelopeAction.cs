using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.WorkFlow.SDK.Tools.Other;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Send
{
    public class SendEnvelopeAction : CustomAction<SendEnvelopeActionConfig>
    {
        StringBuilder _log = new StringBuilder();

        public override void Run(RunCustomActionParams args)
        {         
            try
            {
                var att = AttachmentHelper.GetAttachment(args.Context, Configuration.AttConfig, _log);
                var attContent = Convert.ToBase64String(att.Content);
                var userList = args.Context.CurrentDocument.ItemsLists.GetByID(Configuration.Users.SignersList.ItemListId);

                var skribble = new SkribbleHelper(_log, Configuration.ApiConfig);
                var response = skribble.SendEnvelope(attContent, GenerateRequestModel(), PrepareUsers(userList));

                args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.GuidFildId, response.document_id);
                args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.EnvelopeFildId, response.id);
                args.Context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AttTechnicalFieldID, att.ID);

                SaveUrlForUsers(userList, response);
            }
            catch(Exception e)
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

        private SendEnvelopeProperties GenerateRequestModel()
        {
            return new SendEnvelopeProperties()
            {
                MailSubject = Configuration.MessageContent.MailSubject,
                MailBody = Configuration.MessageContent.MailBody,
                QType = Configuration.Users.QType,
                LType = Configuration.Users.LType
            };
        }

        private List<RequestSignature> PrepareUsers(ItemsList itemsList)
        {
            _log.AppendLine("Adding signers to Envelope");

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
                    mobile_number = Configuration.Users.PhoneAutorization ? row.GetCellValue(Configuration.Users.SignersList.SignerPhoneNumberColumnID).ToString() : "",
                    provider = Configuration.Users.PhoneAutorization ? Configuration.Users.Provider : "",
                    issuing_country = Configuration.Users.PhoneAutorization ? Configuration.Users.IssuingCountry : ""
                };

                users.Add(user);
            }

            return users;
        }

        private void SaveUrlForUsers(ItemsList itemsList, SendResponse response)
        {
            for (int i = 0; i < itemsList.Rows.Count; i++)
            {
                itemsList.Rows.First().SetCellValue(Configuration.ResponseParams.UrlColName, response.signatures[i]?.signing_url ?? response.signing_url);
            }
        }
    }
}