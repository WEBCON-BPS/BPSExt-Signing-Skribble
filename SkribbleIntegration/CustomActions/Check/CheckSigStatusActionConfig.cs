using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Check
{
    public class CheckSigStatusActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Scribble API Settings")]
        public ApiConfiguration ApiConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Input parameters")]
        public InParams RequestParams { get; set; }

        [ConfigGroupBox(DisplayName = "Output parameters")]
        public OutParams ResponseParams { get; set; }
    }

    public class InParams
    {
        [ConfigEditableFormFieldID(DisplayName = "Envelope ID", Description = "Select the text field where the Envelope ID was saved")]
        public int EnvelopeIdFielId { get; set; }
    }

    public class OutParams
    {
        [ConfigEditableFormFieldID(DisplayName = "Copy Document ID to field", Description = "Specify a text field on the form where Document ID will be saved")]
        public int GuidFielId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy Status to field", Description = "Specify a text field on the form where Documents status will be saved")]
        public int StatusFieldId { get; set; }
    }
}
