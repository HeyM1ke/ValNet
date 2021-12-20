using RestSharp;
using ValNet.Interfaces;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Requests;
using System.Net;

namespace ValNet;

/// <summary>
/// Default Account Object.
/// </summary>
public class RiotUser
{
    internal RiotLoginData loginData;
    public RiotRegion UserRegion;
    public RiotUrl _riotUrl = new RiotUrl();
    public RiotTokens tokenData = new RiotTokens();
    public RiotUserData? UserData;
    
    /// <summary>
    /// Signifies Authentication Method used.
    /// </summary>
    public AuthType AuthType;
    
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

    /// <summary>
    /// Class used to interact with Player's Store
    /// </summary>
    public Store Store;
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
        UserCookieJar = new CookieContainer();
        UserClient = new RestClient()
        {
            CookieContainer = UserCookieJar
        };

        Authentication = new (this);
        Requests = new (this);
        Store = new(this);
    }
   

    #region Public Methods

    public void CustomUserRequest(string requestUrl, Method method, string parameters)
    {

    }

    public void ChangeCredentials(RiotLoginData pLoginData){
        this.loginData = pLoginData;
    }

    #endregion
}

public enum AuthType
{
    Cloud,Socket,Cookie
}

public struct RiotUrl
{
    public string glzURL;
    public string pdURL;
}

public struct RiotTokens
{
    public string access;
    public string? entitle;
    public string idToken;
}