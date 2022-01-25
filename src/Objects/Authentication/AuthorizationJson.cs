namespace ValNet.Objects.Authentication;

public class AuthorizationJson
{
    public string type { get; set; }
    public string? error { get; set; }
    public Response? response { get; set; }
    public string country { get; set; }
    public Multifactor? multifactor { get; set; }
    public string? securityProfile { get; set; }
    
    public class Parameters
    {
        public string uri { get; set; }
    }

    public class Response
    {
        public string mode { get; set; }
        public Parameters parameters { get; set; }
    }
    
    public class Multifactor
    {
        public string email { get; set; }
        public string method { get; set; }
        public List<string> methods { get; set; }
        public int multiFactorCodeLength { get; set; }
        public string mfaVersion { get; set; }
    }
}
