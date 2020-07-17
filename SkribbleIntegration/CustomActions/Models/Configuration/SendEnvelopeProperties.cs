namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration
{
    internal class SendEnvelopeProperties
    {
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public QualityType QType { get; set; }
        public LegislationType LType { get; set; }
    }
}
