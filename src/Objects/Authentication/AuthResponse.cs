namespace ValNet.Objects.Authentication;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string EntitlementToken { get; set; }
    
    //Add Expire time, Timestamp
}