using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Send
{
    public class SendEnvelopeActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Scribble API Settings")]
        public ApiConfiguration ApiConfig { get; set; }        

        [ConfigGroupBox(DisplayName = "Attachment selection")]
        public InputAttConfig AttConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Recipients selection")]
        public ItemListConfig Users { get; set; }

        [ConfigGroupBox(DisplayName = "Message content")]
        public MessageContent MessageContent { get; set; }

        [ConfigGroupBox(DisplayName = "Output parameters")]
        public OutParams ResponseParams { get; set; }
    }  

    public class MessageContent
    {
        [ConfigEditableText(DisplayName = "Subject", IsRequired = true)]
        public string MailSubject { get; set; }

        [ConfigEditableText(DisplayName = "Content", IsRequired = true, Multiline = true)]
        public string MailBody { get; set; }
    }

  

    public class ItemListConfig
    {
        [ConfigEditableItemList(DisplayName = "Signers Item List")]
        public SignersList SignersList { get; set; }          

        [ConfigEditableText(DisplayName = "Provider", Description = "Evidence provider where Skribble can verify the signers identity data")]
        public string Provider { get; set; }

        [ConfigEditableText(DisplayName = "Issuing Country", Description = "Issuing country of no account signer")]
        public string IssuingCountry { get; set; }

        [ConfigEditableBool(DisplayName = "Additional SMS verification", Description = "Mark this field if additional verification should be required")]
        public bool PhoneAutorization { get; set; }

        [ConfigEditableEnum(DisplayName = "Signature Quality", Description = "The parameter quality supports following values: https://doc.skribble.com/#signature-quality")]
        public QualityType QType { get; set; }

        [ConfigEditableEnum(DisplayName = "Legislation", Description = @"The legislation parameter takes following values:
ZERTES QES according to the Swiss law.
EIDAS QES according to the EU law")]
        public LegislationType LType { get; set; }
    }

    public class SignersList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "Name", IsRequired = true)]
        public int SignerNameColumnID { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "E-mail", IsRequired = true)]
        public int SignerMailColumnID { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "Phone Number", IsRequired = true)]
        public int SignerPhoneNumberColumnID { get; set; }
    }

    public class OutParams
    {
        [ConfigEditableFormFieldID(DisplayName = "Copy Envelope ID to field", Description = "Specify a text field on the form where Envelope ID will be saved")]
        public int EnvelopeFildId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy Document ID to field", Description = "Specify a text field on the form where Document ID will be saved")]
        public int GuidFildId { get; set; }      

        [ConfigEditableText(DisplayName = "Signing URL", Description = "Specify a text item list column on the form where Signing URI will be saved")]
        public string UrlColName { get; set; }
    } 
}