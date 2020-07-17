using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers
{
    internal class SkribbleHelper
    {        
        private const string AccessEndpoint = "/access/login";
        private const string SignEndpoint = "/signature-requests";
        private readonly string _baseUri, _user, _key, _accesKey;       
        private StringBuilder _log;      

        internal SkribbleHelper(StringBuilder log, ApiConfiguration api)
        {
            _baseUri = api.Uri;
            _user = api.User;
            _key = api.Key;
            _log = log;
            _accesKey = GetToken();          
        }

        private string GetToken()
        {
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUri + AccessEndpoint);
            request.Content = new StringContent($"username={_user}&api-key={_key}", Encoding.UTF8, "application/x-www-form-urlencoded");
            
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }              

        public SendResponse SendEnvelope(string content, SendEnvelopeProperties config, List<RequestSignature> users)
        {
            var requestJson = new SendRequest();
            requestJson.title = config.MailSubject;
            requestJson.message = config.MailBody;
            requestJson.content_type = "application/pdf";
            requestJson.content = content;
            requestJson.signatures = users.ToArray();
            requestJson.quality = config.QType.ToString();
            requestJson.legislation = config.QType == QualityType.QES ? config.LType.ToString() : null;

            var headerJson = JsonConvert.SerializeObject(requestJson, Formatting.None,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}{SignEndpoint}")
            {
                Content = new StringContent(headerJson, Encoding.UTF8, "application/json")
            };

            _log.AppendLine("Sending envelope");
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(result);

            return JsonConvert.DeserializeObject<SendResponse>(result);
        }

        public SendResponse CheckDocumentStatus(string envelopeId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}{SignEndpoint}/{envelopeId}");
            var result = client.SendAsync(request).Result;
            result.EnsureSuccessStatusCode();
            var json = result.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(json);

            return JsonConvert.DeserializeObject<SendResponse>(json);
        }

        public byte[] GetDocumentContent(string docId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));       
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}/documents/{docId}/content");
            var result = client.SendAsync(request).Result;
            result.EnsureSuccessStatusCode();
            var json = result.Content.ReadAsByteArrayAsync().Result;
         
            return json;
        }

        public void DeleteRequest(string envelopeId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));        
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUri}{SignEndpoint}/{envelopeId}");
            var result = client.SendAsync(request).Result;
            result.EnsureSuccessStatusCode();     
        }

        public void DeclineSigRequest(string reqId, string msg)
        {
            var line = new JObject();           
            line.Add("message", JToken.FromObject(msg));          

            var headerJson = JsonConvert.SerializeObject(line, Formatting.Indented);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}{SignEndpoint}/{reqId}/decline")
            {
                Content = new StringContent(headerJson, Encoding.UTF8, "application/json")
            };
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(result);
        }

        public List<Documents> ChecAllkDocumentStatus()
        {
            var uri = $"{_baseUri}{SignEndpoint}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(result);

            return JsonConvert.DeserializeObject<List<Documents>>(result); ;
        }

        public string SendReminder(string docId, string msg, List<RequestSignature> users)
        {
            var line = new JObject();            
            line.Add("message", JToken.FromObject(msg));
            line.Add("id", JToken.FromObject(docId));          
            line.Add("signatures", JToken.FromObject(users.ToArray()));

            var headerJson = JsonConvert.SerializeObject(line, Formatting.Indented);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesKey);
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUri}{SignEndpoint}")
            {
                Content = new StringContent(headerJson, Encoding.UTF8, "application/json")
            };
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(json);

            return JsonConvert.DeserializeObject<SendResponse>(json).document_id;
        }
    }
}
