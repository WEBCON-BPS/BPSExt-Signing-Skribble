using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Reminder
{
    public class SendReminderActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Scribble API Settings")]
        public ApiConfiguration ApiConfig { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Envelope ID", Description = "Select the text field where the Envelope ID was saved.")]
        public int EnvelopeFielId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Status", Description = "Select the text field where the Document status was saved.")]
        public int StatusFielId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy Document ID to field", Description = "Specify a text field on the form where Document ID will be saved")]
        public int GuidFielId { get; set; }

        [ConfigEditableText(DisplayName = "Message content")]
        public string Message { get; set; }

        [ConfigGroupBox(DisplayName = "Recipients selection")]
        public ItemListConfig Users { get; set; }
    }

    public class ItemListConfig
    {
        [ConfigEditableItemList(DisplayName = "Signers Item List")]
        public SignersList SignersList { get; set; }    
    }

    public class SignersList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "Name", IsRequired = true)]
        public int SignerNameColumnID { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "E-mail", IsRequired = true)]
        public int SignerMailColumnID { get; set; }
       
    }
}