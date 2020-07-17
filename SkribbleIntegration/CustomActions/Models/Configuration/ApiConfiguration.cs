using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration
{
    public class ApiConfiguration
    {
        [ConfigEditableText(DisplayName = "Base Uri", IsRequired = true)]
        public string Uri { get; set; }

        [ConfigEditableText(DisplayName = "Username", IsRequired = true)]
        public string User { get; set; }

        [ConfigEditableText(DisplayName = "API Key", IsRequired = true)]
        public string Key { get; set; }
    }

    public enum QualityType
    {
        QES,
        AES,
        AES_MINIMAL
    }

    public enum LegislationType
    {
        ZERTES,
        EIDAS
    }
}
