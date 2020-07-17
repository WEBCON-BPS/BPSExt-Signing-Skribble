namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models
{
    public class ResponseSignature
    {
        public string sid { get; set; }
        public string signer_email_address { get; set; }
        public Signer_Identity_Data signer_identity_data { get; set; }
        public int order { get; set; }
        public string status_code { get; set; }
        public bool notify { get; set; }
        public string signing_url { get; set; }
    }

    public class Signer_Identity_Data
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
