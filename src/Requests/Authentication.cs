using System.Diagnostics;
using System.Security.Authentication;
using System.Text.RegularExpressions;

using RestSharp;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ValNet.Requests;

public class Authentication : RequestBase 
{
    #region Riot Authentication Urls
    private const string authUrl = "https://auth.riotgames.com/api/v1/authorization";
    private const string entitleUrl = "https://entitlements.auth.riotgames.com/api/token/v1";
    private const string userInfoUrl = "https://auth.riotgames.com/userinfo";
    private const string regionUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/product/valorant";
    private const string cookieJson = "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";
    #endregion
    
    #region Authentication Objects

    internal Lockfile userLockfile;
    public WebSocket userWebsocket;
    #endregion
    
    public Authentication(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    #region Authentication Methods
    
    public void AuthenticateWithCloud()
    {
        if (_user.loginData.username == null || _user.loginData.password == null)
            throw new Exception("Username or password are empty, please retry when there are values in place.");

        // get initial cookies for auth
        IRestRequest initCookies = new RestRequest(authUrl, Method.POST);
        initCookies.AddJsonBody(cookieJson);
        _user.UserClient.Execute(initCookies);

        // Auth with user details

        var authData = new
        {
            type = "auth",
            username = _user.loginData.username,
            password = _user.loginData.password,
            remember = "true"
        };

        IRestRequest loginRequest = new RestRequest(authUrl, Method.PUT);
        loginRequest.AddJsonBody(JsonSerializer.Serialize(authData));
        var resp = _user.UserClient.Execute(loginRequest);

        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");

        AuthorizationJson? authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj.error is not null)
        {
            throw new Exception("Username/Password is not correct, please check again.");
        }
        
        if (authObj is null || authObj.response is null)
            throw new Exception("Could not properly authenticate.");


        ParseWebToken(authObj.response.parameters.uri);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);
        
        _user.UserData = GetUserData();
        _user.UserRegion = GetUserRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Cloud;
    }
    
    public void AuthenticateWithSocket()
    {
        //Check for Lockfile
        if (ParseLockFile() == false)
            throw new Exception("Game is not Open.");
        
        //Lockfile needs to excist for the remaining methods need to run.
        ConnectToWebsocket(); // Connects to the websocket

        IRestRequest sockToken = new RestRequest($"https://127.0.0.1:{userLockfile.port}/entitlements/v1/token", Method.GET);
        
        sockToken.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{_user.Authentication.userLockfile.password}"))}");
        
        //Move this!
        sockToken.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        
        // Change This!
        sockToken.AddHeader("X-Riot-ClientVersion", "release-04.00-shipping-20-655657"); 
        
        var resp = new RestClient()
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        }.Execute(sockToken);
        
        if(!resp.IsSuccessful)
            throw new Exception("Error reaching game.");
        var tokens = JsonSerializer.Deserialize<WebsocketTokens>(resp.Content);

        _user.tokenData.entitle = tokens.token;
        _user.tokenData.access = tokens.accessToken;

        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = GetUserData();
        WebsocketDetermineRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Socket;
    }
    
    public void AuthenticateWithCookies()
    {
        IRestRequest initCookies = new RestRequest(authUrl, Method.POST);
        initCookies.AddJsonBody(cookieJson);
        var resp = _user.UserClient.Execute(initCookies);
        
        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");

        AuthorizationJson? authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj is null || authObj.response is null)
            throw new Exception("Could not properly authenticate.");


        ParseWebToken(authObj.response.parameters.uri);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);
        
        _user.UserData = GetUserData();

        _user.UserRegion = GetUserRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Cookie;
    }

    #endregion

    #region Cloud/Cookie Methods

    void ParseWebToken(string tokenURL)
    {
       _user.tokenData.access = Regex.Match(tokenURL, @"access_token=(.+?)&scope=").Groups[1].Value;
       _user.tokenData.idToken = Regex.Match(tokenURL, @"id_token=(.+?)&token_type=").Groups[1].Value;
    }
    
    private string? GetEntitlementToken()
    {
        RestRequest request = new RestRequest(entitleUrl, Method.POST);
        
        request.AddJsonBody("{}");

        var resp = _user.UserClient.Execute(request);

        if (!resp.IsSuccessful)
            throw new Exception("Failed to get entitlement token.");
        
        var entitlement_token = JObject.FromObject(JsonConvert.DeserializeObject(resp.Content));

        return (entitlement_token["entitlements_token"] ?? throw new InvalidOperationException("Entitlement Token not Found in Request.")).Value<string>();
    }

    private RiotUserData? GetUserData()
    {
        IRestRequest userDataReq = new RestRequest(userInfoUrl, Method.GET);
        var resp = _user.UserClient.Execute(userDataReq);
        if (!resp.IsSuccessful)
            throw new Exception("Failed to get UserData");
        
        return JsonConvert.DeserializeObject<RiotUserData>(resp.Content);

    }

    private RiotRegion GetUserRegion(string region = null)
    {
        string liveRegion = "";
        if (region is null)
        {
            IRestRequest request = new RestRequest(regionUrl, Method.PUT);
            var idTokData = new
            {
                id_token = _user.tokenData.idToken
            };
            request.AddJsonBody(JsonSerializer.Serialize(idTokData));
            var resp = _user.UserClient.Execute(request);
            if (!resp.IsSuccessful)
                throw new Exception("Failed to get user region.");

            JToken authObj = JObject.FromObject(JsonConvert.DeserializeObject(resp.Content));

            liveRegion = authObj["affinities"]["live"].Value<string>();
        }
        else
            liveRegion = region;

        switch (liveRegion)
        {
            case"na":
                _user._riotUrl.glzURL = "https://glz-na-1.na.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.na.a.pvp.net";
                return RiotRegion.NA;
            case "eu":
                _user._riotUrl.glzURL = "https://glz-eu-1.eu.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.eu.a.pvp.net";
                return RiotRegion.EU;
            case"kr":
                _user._riotUrl.glzURL = "https://glz-kr-1.kr.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.kr.a.pvp.net";
                return RiotRegion.KR;
            case "latam":
                _user._riotUrl.glzURL = "https://glz-latam-1.na.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.na.a.pvp.net";
                return RiotRegion.LATAM;
            case"br":
                _user._riotUrl.glzURL = "https://glz-br-1.na.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.na.a.pvp.net";
                return RiotRegion.BR;
            case "ap":
                _user._riotUrl.glzURL = "https://glz-ap-1.ap.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.ap.a.pvp.net";
                return RiotRegion.AP;
            default:
                return RiotRegion.NA;

        }
    }
    #endregion
    
    #region Websocket Methods
    bool ParseLockFile()
    {
        var lockfileLocation =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

        if (File.Exists(lockfileLocation))
        {
            using (FileStream fileStream = new FileStream(lockfileLocation, FileMode.Open, FileAccess.ReadWrite,
                       FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fileStream))
            {
                string[] parts = sr.ReadToEnd().Split(":");

                userLockfile.processName = parts[0];
                userLockfile.processId = parts[1];
                userLockfile.port = parts[2];
                userLockfile.password = parts[3];
                userLockfile.protocol = parts[4];

                return true;
            }
        }
        // Lock File was not found, wait for the file to show up using a watcher.
        return false;
    }

    void ConnectToWebsocket()
    {
        userWebsocket = new WebSocket($"wss://127.0.0.1:{userLockfile.port}/", "wamp");
        userWebsocket.SetCredentials("riot", userLockfile.password, true);
        userWebsocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
        userWebsocket.SslConfiguration.ServerCertificateValidationCallback = delegate { return true; };
        userWebsocket.Connect();
        userWebsocket.Send("[5, \"OnJsonApiEvent\"]");
    }

    async void WebsocketDetermineRegion()
    {
        var valorantData = new
        {
            product = "valorant"
        };
        var data = await _user.Requests.WebsocketRequest("/player-affinity/product/v1/token", Method.POST, "", valorantData);
        
        var Affinities = JsonSerializer.Deserialize<WebsocketAffinities>((string)data.content);

        _user.UserRegion = GetUserRegion(Affinities.affinities.live);

    }
    #endregion

    #region General Methods

    void GetCurrentGameVersion()
    {
        // Method adds client ver header to both clients

        var resp = JsonSerializer.Deserialize<ValorantApi_VersionResp>(new RestClient()
            .ExecuteAsync(new RestRequest("https://valorant-api.com/v1/version", Method.GET)).Result.Content);

        _user.UserClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
        _user.SocketClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
    }

    #endregion
}