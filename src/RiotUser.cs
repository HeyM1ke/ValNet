using RestSharp;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Requests;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using WebSocketSharp;

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
    public RestClient UserClient { get; set; }
    
    /// <summary>
    /// User HTTPClient used for WebSocket requests
    /// </summary>
    public RestClient SocketClient { get; set; }
    
    /// <summary>
    /// Websocket.
    /// </summary>
    public WebSocket UserWebsocket;

    /// <summary>
    /// CookieContainer used to hold User's Cookies
    /// </summary>
    public string lmaoTest;
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
    /// Class used to intereact with the Player's Contact Progess.
    /// </summary>
    public Contracts Contracts;

    public Player Player;
    
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


    private void UserSetup()
    {
        var optionsWebClient = new RestClientOptions()
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            UserAgent = "RiotClient/43.0.1.4195386.4190634 rso-auth (Windows;10;;Professional, x64)"
        };
        ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls11;
        var optionsClient = new RestClientOptions()
        {
            UserAgent = "RiotClient/43.0.1.4195386.4190634 rso-auth (Windows;10;;Professional, x64)"
        };
        UserClient = new RestClient(optionsClient);
        UserClient.AddDefaultHeader("X-Riot-ClientPlatform",
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        SocketClient = new RestClient(optionsWebClient);
        
        Authentication = new (this);
        Requests = new (this);
        Store = new(this);
        Inventory = new(this);
        Party = new(this);
        Contracts = new(this);
        Player = new(this);
    }
   

    #region Public Methods
    
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
        var requestResp = await this.Requests.RiotGlzRequest($"/session/v1/sessions/{this.UserData.sub}", Method.Get);

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