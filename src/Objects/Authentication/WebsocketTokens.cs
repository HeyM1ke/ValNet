namespace ValNet.Objects.Authentication;

public class WebsocketTokens
{
    public string accessToken { get; set; }
    public List<string> entitlements { get; set; }
    public string issuer { get; set; }
    public string subject { get; set; }
    public string token { get; set; }
}