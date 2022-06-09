using RestSharp;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Requests;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Authentication;
using System.Text.Json;
using WebSocketSharp;

namespace ValNet;

/// <summary>
/// Default Account Object.
/// </summary>
public class RiotUser
{
    internal RiotLoginData loginData;
    public RiotRegion UserRegion = RiotRegion.UNKNOWN;
    public RiotUrl _riotUrl = new();
    public RiotTokens tokenData = new();
    public RiotUserData? UserData;

    /// <summary>
    /// Signifies Authentication Method used.
    /// </summary>
    public AuthType AuthType;

    public RequestClient AuthClient { get; set; }
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
    public bool IsPlayerInGame => CheckPlayerInGame().Result;

    public string UserAgentFormat = "RiotClient/51.0.0.4429735.4381201 {0} (Windows;10;;Professional, x64)";
    public RiotUser()
    {
        UserSetup();
    }

    public RiotUser(string userAgentFormat)
    {
        UserAgentFormat = userAgentFormat;
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

        UserRegion = pRegion; // Set UserRegion instance Variable to value of parameter region

        UserSetup();
    }


    private void UserSetup()
    {
        var optionsWebClient = new RestClientOptions()
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            UserAgent = string.Format(UserAgentFormat, "rso-auth")
        };
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var optionsClient = new RestClientOptions()
        {
            UserAgent = string.Format(UserAgentFormat, "rso-auth")
        };
        
        AuthClient = new RequestClient();
        
        UserClient = new RestClient(optionsClient);
       
        SocketClient = new RestClient(optionsWebClient);

        Authentication = new Authentication(this);
        Requests = new RequestBase(this);
        Store = new Store(this);
        Inventory = new Inventory(this);
        Party = new Party(this);
        Contracts = new Contracts(this);
        Player = new Player(this);
    }


    #region Public Methods

    public void ChangeCredentials(RiotLoginData pLoginData)
    {
        loginData = pLoginData;
    }

    public void SetupParty()
    {
        if (!IsPlayerInGame)
            throw new Exception("User is not in game, please try when the user is in game.");

        Party.InitialPartySetup();
    }

    #endregion

    #region Private Methods

    internal async Task<bool> CheckPlayerInGame()
    {
        var requestResp = await Requests.RiotGlzRequest($"/session/v1/sessions/{UserData.sub}", Method.Get);

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
    Cloud,
    Socket,
    Cookie
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
    public string? pasToken;
}
