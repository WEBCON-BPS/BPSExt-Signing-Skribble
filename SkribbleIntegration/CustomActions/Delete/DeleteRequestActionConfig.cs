using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Delete
{
    public class DeleteRequestActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Scribble API Settings")]
        public ApiConfiguration ApiConfig { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Envelope ID", Description = "Select the text field where the Envelope ID was saved")]
        public int EnvelopeFielId { get; set; }

        [ConfigEditableBool(DisplayName = "Delete/Decline", Description = "true - delete, false - decline", DefaultValue = false)]
        public bool DeleteOperationType { get; set; }

        [ConfigEditableText(DisplayName = "Message content", Description = "Add your reason for declining")]
        public string Message { get; set; }
    } 
}