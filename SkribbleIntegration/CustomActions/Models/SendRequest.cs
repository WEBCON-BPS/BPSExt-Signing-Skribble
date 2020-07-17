namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models
{
    public class SendRequest
    {
        public string title { get; set; }
        public string message { get; set; }
        public string content { get; set; }
        public string content_type { get; set; }
        public string file_url { get; set; }
        public string document_id { get; set; }
        public string legislation { get; set; }
        public string quality { get; set; }
        public RequestSignature[] signatures { get; set; }
        public string[] cc_email_addresses { get; set; }
        public string callback_success_url { get; set; }
        public string callback_error_url { get; set; }
        public string callback_update_url { get; set; }
        public string custom { get; set; }
        public string[] write_access { get; set; }
    }

    public class RequestSignature
    {
        public string signer_email_address { get; set; }
        public SignerIdentityData signer_identity_data { get; set; }
        public string signing_url { get; set; }
    }

    public class SignerIdentityData
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string provider { get; set; }
        public string mobile_number { get; set; }
        public string issuing_country { get; set; }
    }
}
