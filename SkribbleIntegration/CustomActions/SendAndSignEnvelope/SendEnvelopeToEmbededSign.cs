using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Tools.Other;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.SendAndSignEnvelope
{
    public class SendEnvelopeToEmbededSign : CustomAction<SendEnvelopeToEmbededSignConfig>
    {
        StringBuilder _log = new StringBuilder();

        public override void Run(RunCustomActionParams args)
        {            
            try
            {
                var att = AttachmentHelper.GetAttachment(args.Context, Configuration.AttConfig, _log);
                var attContent = Convert.ToBase64String(att.Content);               
                var response = new SkribbleHelper(_log, Configuration.ApiConfig).SendEnvelope(attContent, GenerateRequestModel(), PrepareUser());

                args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.GuidFildId, response.document_id);
                args.Context.CurrentDocument.SetFieldValue(Configuration.ResponseParams.EnvelopeFildId, response.id);
                args.Context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AttTechnicalFieldID, att.ID);

                args.TransitionInfo.RedirectUrl($"{response.signing_url}?exitURL={Configuration.RedirectUrl}");
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

        private List<RequestSignature> PrepareUser()
        {
            _log.AppendLine("Adding signer to Envelope");

            var users = new List<RequestSignature>();

            var user = new RequestSignature();
            user.signer_email_address = Configuration.Users.SignerMail;
            user.signer_identity_data = new SignerIdentityData()
            {
                first_name = TextHelper.GetPairName(Configuration.Users.SignerName).Split(' ').First(),
                last_name = TextHelper.GetPairName(Configuration.Users.SignerName).Split(' ').Last(),
                mobile_number = Configuration.Users.PhoneAutorization ? Configuration.Users.SignerPhoneNumber : "",
                provider = Configuration.Users.PhoneAutorization ? Configuration.Users.Provider : "",
                issuing_country = Configuration.Users.PhoneAutorization ? Configuration.Users.IssuingCountry : ""
            };

            users.Add(user);

            return users;
        }     
    }
}