namespace ValNet.Objects.Authentication;

public class AuthenticationStatus
{
    public int ValNetCode { get; set; }

    public bool bIsAuthComplete;
    public string type { get; set; }
    public string error { get; set; }
    public AuthorizationJson.Multifactor multifactorData { get; set; }
}