namespace ValNet.Objects.Authentication;

public class AuthorizationJson
{
    public string type { get; set; }
    public string error { get; set; }
    public Response response { get; set; }
    public string country { get; set; }
    
    public class Parameters
    {
        public string uri { get; set; }
    }

    public class Response
    {
        public string mode { get; set; }
        public Parameters parameters { get; set; }
    }
}