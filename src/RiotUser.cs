using RestSharp;
using ValNet.Interfaces;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Requests;
using System.Net;
using System.Text.Json;

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
    /// User HTTPClient used for Web requests
    /// </summary>
    internal RestClient UserClient { get; set; }
    
    /// <summary>
    /// User HTTPClient used for WebSocket requests
    /// </summary>
    internal RestClient SocketClient { get; set; }
    
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

    /// <summary>
    /// Class used to interact with Player's Inventory
    /// </summary>
    public Inventory Inventory;

    /// <summary>
    /// Class used to intereact with the Player's Party (Needs to be in-game) to use.
    /// </summary>
    public Party Party;

    /// <summary>
    /// Returns true if player is in game.
    /// </summary>
    public bool IsPlayerInGame {
        get
        {
            return CheckPlayerInGame().Result;
        }
    }
    
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
        UserClient = new RestClient
        {
            CookieContainer = UserCookieJar
        };

        SocketClient = new RestClient
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };

        
        Authentication = new (this);
        Requests = new (this);
        Store = new(this);
        Inventory = new(this);
        Party = new(this);
    }
   

    #region Public Methods
    
    public void CustomUserRequest(string requestUrl, Method method, string parameters)
    {

    }

    public void ChangeCredentials(RiotLoginData pLoginData){
        this.loginData = pLoginData;
    }
    
    public void SetupParty()
    {
        if (!IsPlayerInGame)
            throw new Exception("User is not in game, please try when the user is in game.");
        
        this.Party.InitialPartySetup();
    }

    #endregion

    #region Private Methods

    internal async Task<bool> CheckPlayerInGame()
    {
        var requestResp = await this.Requests.RiotGlzRequest($"/session/v1/sessions/{this.UserData.sub}", Method.GET);

        if (!requestResp.isSucc)
            return false;

        var obj = JsonSerializer.Deserialize<RiotSessionObj>(requestResp.content.ToString());

        if (obj.cxnState.Equals("CONNECTED"))
            return true;

        
        return false;
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
    public string? access;
    public string? entitle;
    public string? idToken;
}