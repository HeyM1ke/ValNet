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
    internal RiotRegion UserRegion;
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

    /// <summary>
    /// Class that can be used to make request
    /// </summary>
    /// 
    public RequestBase Requests;

    public RiotUser()
    {
        UserSetup();
    }
    public RiotUser(RiotLoginData pLoginData)
    {
        loginData = pLoginData; // Set Logindata instance Variable to value of parameter logindata

        UserSetup();
    }
    public RiotUser(RiotLoginData pLoginData, RiotRegion pRegion)
    {
        loginData = pLoginData; // Set Logindata instance Variable to value of parameter logindata
        
        UserRegion = pRegion;   // Set UserRegion instance Variable to value of parameter region

        UserSetup();
    }


    private void UserSetup(){
        UserClient = new RestClient()
        {
            CookieContainer = UserCookieJar
        };

        Authentication = new Authentication(this);
        RequestBase = new RequestBase(this);
    }
   

    #region Public Methods

    void CustomUserRequest(string requestUrl, Method method, string parameters)
    {

    }

    void ChangeCredentials(RiotLoginData pLoginData){
        this.loginData = pLoginData;
    }

    #endregion
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