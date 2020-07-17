using System;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models
{
    public class StatusResponse
    {
        public Documents[] Property1 { get; set; }
    }

    public class Documents
    {
        public string id { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public string document_id { get; set; }
        public string quality { get; set; }
        public string signing_url { get; set; }
        public string status_overall { get; set; }
        public ResponseSignature[] signatures { get; set; }
        public object[] cc_email_addresses { get; set; }
        public string owner { get; set; }
        public string[] read_access { get; set; }
        public string[] write_access { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }   
}
