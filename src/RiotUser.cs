using RestSharp;
using ValNet.Interfaces;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Requests;

namespace ValNet;

/// <summary>
/// Default Account Object.
/// </summary>
public class RiotUser
{
    internal RiotLoginData loginData;
    private RiotRegion UserRegion;
    internal RiotUrl _riotUrl = new RiotUrl();
    /// <summary>
    /// Signifies Authentication Method used.
    /// </summary>
    internal AuthType AuthType;
    
    /// <summary>
    /// User HTTPClient used for requests
    /// </summary>
    internal RestClient UserClient { get; set; }
    
    /// <summary>
    /// CookieContainer used to hold User's Cookies
    /// </summary>
    public CookieContainer UserCookieJar { get; set; }
    
    /// <summary>
    /// Authentication class that is used to authenticate user.
    /// </summary>
    /// 
    public Authentication Authentication;
    
    public RiotUser()
    {
        UserClient = new RestClient()
        {
            CookieContainer = UserCookieJar
        };
        Authentication = new Authentication(this);
    }
    public RiotUser(RiotLoginData loginData)
    {
        UserClient = new RestClient()
        {
            CookieContainer = UserCookieJar
        };
        Authentication = new Authentication(this);
    }
    public RiotUser(RiotLoginData pLoginData, RiotRegion pRegion)
    {
        UserClient = new RestClient()
        {
            CookieContainer = UserCookieJar,
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        
        //Create User Module Instance Classes
        Authentication = new Authentication(this);
        
        loginData = pLoginData; // Set Logindata instance Variable to value of parameter logindata
        
        UserRegion = pRegion;   // Set UserRegion instance Variable to value of parameter region
    }
}

internal enum AuthType
{
    Cloud,Socket,Cookie
}

internal struct RiotUrl
{
    public string glzURL;
    public string pdURL;
}