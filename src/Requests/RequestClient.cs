using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace ValNet.Requests
{
    public class RequestClient
    {
        public readonly HttpClient Client;
        private readonly HttpClientHandler _handler;
        public CookieContainer CookieContainer;

        private string _userAgent = "RiotClient/43.0.1.4195386.4190634 rso-auth (Windows; 10;;Professional, x64)";
        
        public RequestClient()
        {
            CookieContainer = new CookieContainer();
            _handler = new HttpClientHandler() 
            {
                UseCookies = true,
                SslProtocols = SslProtocols.Tls12,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                CookieContainer = CookieContainer
            };
            
            
            
            Client = new HttpClient(_handler)
            {
                DefaultRequestVersion = HttpVersion.Version20
            };
            
            Client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
        }
        public async Task<HttpResponseMessage> GetAsync(string RequestUrl)
        {
            var r = await Client.GetAsync(RequestUrl);
            return r;
        }
        public async Task<HttpResponseMessage> PostAsync(string RequestUrl, object Data = null )
        {
            StringContent c;
            if (Data != null)
            {
                var json = JsonSerializer.Serialize(Data);
                c = new StringContent(json, Encoding.UTF8, "application/json");    
            }
            else
                c = new StringContent("", Encoding.UTF8, "application/json");    
            
            var r = await Client.PostAsync(RequestUrl,c);
            return r; 
        }
        public async Task<HttpResponseMessage> PutAsync(string RequestUrl, object Data = null )
        {
            StringContent c;
            if (Data != null)
            {
                var json = JsonSerializer.Serialize(Data);
                c = new StringContent(json, Encoding.UTF8, "application/json");    
            }
            else
                c = new StringContent("", Encoding.UTF8, "application/json"); 
            
            var r = await Client.PutAsync(RequestUrl, c);
            return r; 
        }
        public async Task<HttpResponseMessage> DelAsync(string RequestUrl)
        {
            var r = await Client.DeleteAsync(RequestUrl);
            return r; 
        }

        public CookieContainer GetClientCookies => _handler.CookieContainer;
        public void AddHeaderToClient(string key, string value) => Client.DefaultRequestHeaders.Add(key, value);
    }
}
