namespace ValNet.Objects.Authentication;

public class RSOWebsocketObj
{
    public Authorization authorization { get; set; }
    public string country { get; set; }
    public string type { get; set; }
    
    public class AccessToken
    {
        public int expiry { get; set; }
        public List<string> scopes { get; set; }
        public string token { get; set; }
    }

    public class IdToken
    {
        public int expiry { get; set; }
        public string token { get; set; }
    }

    public class Authorization
    {
        public AccessToken accessToken { get; set; }
        public IdToken idToken { get; set; }
    }

}