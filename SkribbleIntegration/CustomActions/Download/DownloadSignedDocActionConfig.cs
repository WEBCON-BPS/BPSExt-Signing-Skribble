using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Download
{
    public class DownloadSignedDocActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Scribble API Settings")]
        public ApiConfiguration ApiConfig { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved")]
        public int GuidIdFielId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Status", Description = "Select the text field where the Document status was saved")]
        public int StatusFielId { get; set; }

        [ConfigGroupBox(DisplayName = "Attachment selection")]
        public AttConfig AttConfig { get; set; }       
    }    

    public class AttConfig
    {
        [ConfigEditableText(DisplayName = "Suffix", Description = "Suffix that will be added to the name of the downloaded file. When this field is empty then the attachment will be overwritten (if the attachment with the selected Document ID exists on the form).", DefaultText = "_sign")]
        public string AttSufix { get; set; }

        [ConfigEditableText(DisplayName = "Category", Description = "Attachment category where the signed documents will be downloaded")]
        public string SaveCategory { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy attachment ID to field", Description = "Select the technical field where the source attachment ID was saved.")]
        public int AttTechnicalFieldID { get; set; }
    }
}