using System.Security.Authentication;
using System.Text.RegularExpressions;
using RestSharp;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using WebSocketSharp;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ValNet.Requests;

public class Authentication : RequestBase
{
    #region Riot Authentication Urls

    private const string authUrl = "https://auth.riotgames.com/api/v1/authorization";
    private const string entitleUrl = "https://entitlements.auth.riotgames.com/api/token/v1";
    private const string userInfoUrl = "https://auth.riotgames.com/userinfo";
    private const string regionUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/product/valorant";

    private const string cookieJson =
        "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" +
        "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";

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

    public async Task<AuthenticationStatus> AuthenticateWithCloud()
    {
        if (_user.loginData.username == null || _user.loginData.password == null)
            throw new Exception("Username or password are empty, please retry when there are values in place.");

        // get initial cookies for auth
        RestRequest initCookies = new RestRequest(authUrl, Method.Post);
        initCookies.AddJsonBody(cookieJson);
        _user.UserClient.ExecuteAsync(initCookies);

        // Auth with user details

        var authData = new
        {
            type = "auth",
            username = _user.loginData.username,
            password = _user.loginData.password,
            remember = "true"
        };

        RestRequest loginRequest = new RestRequest(authUrl, Method.Put);
        loginRequest.AddJsonBody(JsonSerializer.Serialize(authData));
        var resp = await _user.UserClient.ExecuteAsync(loginRequest);

        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");

        var authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj.error is not null) throw new Exception("Error: Username/Password is incorrect");

        // Two Factor Authentication Block

        //Determine if Two Factor is needed
        if (authObj.type.Equals("multifactor"))
            return new AuthenticationStatus()
            {
                bIsAuthComplete = false,
                type = "multifactor",
                multifactorData = authObj.multifactor
            };
        
        
        return await CompleteAuth(authObj);


        // End
    }

    public async Task<AuthenticationStatus> AuthenticateTwoFactorCode(string code)
    {
        RestRequest multifactorRequest = new RestRequest(authUrl, Method.Put);
        var data = new
        {
            type = "multifactor",
            code = code,
            rememberDevice = true
        };

        multifactorRequest.AddJsonBody(JsonSerializer.Serialize(data));
        var resp = await _user.UserClient.ExecuteAsync(multifactorRequest);

        if (!resp.IsSuccessful)
            throw new Exception("An Error has Occurred");

        var authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj.error is not null && authObj.error.Equals("multifactor_attempt_failed"))
            return new AuthenticationStatus()
            {
                bIsAuthComplete = false,
                type = authObj.type,
                multifactorData = authObj.multifactor,
                error = authObj.error
            };

        if (authObj.type.Equals("response")) return await CompleteAuth(authObj);

        throw new Exception("Unknown Error has occured.");
    }

    private async Task<AuthenticationStatus> CompleteAuth(AuthorizationJson authObj)
    {
        ParseWebToken(authObj.response.parameters.uri);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = await GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = await GetUserData();
        _user.UserRegion = await GetUserRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Cloud;

        return new AuthenticationStatus()
        {
            bIsAuthComplete = true
        };
    }

    public async void AuthenticateWithToken(string redirectUrl)
    {
        // get initial cookies for auth
        RestRequest initCookies = new RestRequest(authUrl, Method.Post);
        initCookies.AddJsonBody(cookieJson);
        await _user.UserClient.ExecuteAsync(initCookies);

        ParseWebToken(redirectUrl);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = await GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = await GetUserData();
        _user.UserRegion = await GetUserRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Cloud;
    }

    public async void AuthenticateWithSocket()
    {
        //Check for Lockfile
        if (ParseLockFile() == false)
            throw new Exception("Game is not Open.");

        //Lockfile needs to excist for the remaining methods need to run.
        ConnectToWebsocket(); // Connects to the websocket

        RestRequest sockToken =
            new RestRequest($"https://127.0.0.1:{userLockfile.port}/entitlements/v1/token", Method.Get);

        sockToken.AddHeader("Authorization",
            $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{_user.Authentication.userLockfile.password}"))}");

        //Move this!
        sockToken.AddHeader("X-Riot-ClientPlatform",
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");

        // Change This!
        sockToken.AddHeader("X-Riot-ClientVersion", "release-04.00-shipping-20-655657");

        var resp = await _user.SocketClient.ExecuteAsync(sockToken);

        if (!resp.IsSuccessful)
            throw new Exception("Error reaching game.");
        var tokens = JsonSerializer.Deserialize<WebsocketTokens>(resp.Content);

        _user.tokenData.entitle = tokens.token;
        _user.tokenData.access = tokens.accessToken;

        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = await GetUserData();
        WebsocketDetermineRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Socket;
    }

    public async void AuthenticateWithCookies()
    {
        RestRequest initCookies = new RestRequest(authUrl, Method.Post);
        initCookies.AddJsonBody(cookieJson);
        var resp = await _user.UserClient.ExecuteAsync(initCookies);

        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");

        var authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj is null || authObj.response is null)
            throw new Exception("Could not properly authenticate.");


        ParseWebToken(authObj.response.parameters.uri);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = await GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = await GetUserData();

        _user.UserRegion = await GetUserRegion();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Cookie;
    }

    #endregion

    #region Cloud/Cookie Methods

    private void ParseWebToken(string tokenURL)
    {
        _user.tokenData.access = Regex.Match(tokenURL, @"access_token=(.+?)&scope=").Groups[1].Value;
        _user.tokenData.idToken = Regex.Match(tokenURL, @"id_token=(.+?)&token_type=").Groups[1].Value;
    }

    private async Task<string?> GetEntitlementToken()
    {
        var request = new RestRequest(entitleUrl, Method.Post);

        request.AddJsonBody("{}");

        var resp = await _user.UserClient.ExecuteAsync(request);

        if (!resp.IsSuccessful)
            throw new Exception("Failed to get entitlement token.");

        
        // Testing new Json Parsing 
        string entitlement_token = "";
        var respJson = JsonDocument.Parse(resp.Content);
        if (respJson.RootElement.TryGetProperty("entitlements_token", out JsonElement tokenElement))
        {
            entitlement_token = tokenElement.GetString();
        }


        return entitlement_token;
    }
 
    private async Task<RiotUserData?> GetUserData()
    {
        RestRequest userDataReq = new RestRequest(userInfoUrl, Method.Get);
        var resp = await _user.UserClient.ExecuteAsync(userDataReq);
        if (!resp.IsSuccessful)
            throw new Exception("Failed to get UserData");

        return JsonSerializer.Deserialize<RiotUserData>(resp.Content);
    }

    private async Task<RiotRegion> GetUserRegion(string region = null)
    {
        var liveRegion = "";
        if (region is null)
        {
            RestRequest request = new RestRequest(regionUrl, Method.Put);
            var idTokData = new
            {
                id_token = _user.tokenData.idToken
            };
            request.AddJsonBody(JsonSerializer.Serialize(idTokData));
            var resp = await _user.UserClient.ExecuteAsync(request);
            if (!resp.IsSuccessful)
                throw new Exception("Failed to get user region.");
            
            JsonNode respNode = JsonNode.Parse(resp.Content);
            var authObj = respNode["affinities"]["live"];

            liveRegion = authObj.ToJsonString();
        }
        else
        {
            liveRegion = region;
        }

        switch (liveRegion)
        {
            case "na":
                _user._riotUrl.glzURL = "https://glz-na-1.na.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.na.a.pvp.net";
                return RiotRegion.NA;
            case "eu":
                _user._riotUrl.glzURL = "https://glz-eu-1.eu.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.eu.a.pvp.net";
                return RiotRegion.EU;
            case "kr":
                _user._riotUrl.glzURL = "https://glz-kr-1.kr.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.kr.a.pvp.net";
                return RiotRegion.KR;
            case "latam":
                _user._riotUrl.glzURL = "https://glz-latam-1.na.a.pvp.net";
                _user._riotUrl.pdURL = "https://pd.na.a.pvp.net";
                return RiotRegion.LATAM;
            case "br":
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

    private bool ParseLockFile()
    {
        var lockfileLocation =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

        if (File.Exists(lockfileLocation))
            using (var fileStream = new FileStream(lockfileLocation, FileMode.Open, FileAccess.ReadWrite,
                       FileShare.ReadWrite))
            using (var sr = new StreamReader(fileStream))
            {
                var parts = sr.ReadToEnd().Split(":");

                userLockfile.processName = parts[0];
                userLockfile.processId = parts[1];
                userLockfile.port = parts[2];
                userLockfile.password = parts[3];
                userLockfile.protocol = parts[4];

                return true;
            }

        // Lock File was not found, wait for the file to show up using a watcher.
        return false;
    }

    private void ConnectToWebsocket()
    {
        userWebsocket = new WebSocket($"wss://127.0.0.1:{userLockfile.port}/", "wamp");
        userWebsocket.SetCredentials("riot", userLockfile.password, true);
        userWebsocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
        userWebsocket.SslConfiguration.ServerCertificateValidationCallback = delegate { return true; };
        userWebsocket.Connect();
        userWebsocket.Send("[5, \"OnJsonApiEvent\"]");
    }

    private async void WebsocketDetermineRegion()
    {
        var valorantData = new
        {
            product = "valorant"
        };
        var data = await _user.Requests.WebsocketRequest("/player-affinity/product/v1/token", Method.Post, "",
            valorantData);

        var Affinities = JsonSerializer.Deserialize<WebsocketAffinities>((string) data.content);

        _user.UserRegion = await GetUserRegion(Affinities.affinities.live);
    }

    #endregion

    #region General Methods

    private void GetCurrentGameVersion()
    {
        // Method adds client ver header to both clients

        var resp = JsonSerializer.Deserialize<ValorantApi_VersionResp>(new RestClient()
            .ExecuteAsync(new RestRequest("https://valorant-api.com/v1/version", Method.Get)).Result.Content);

        _user.UserClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
        _user.SocketClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
    }

    #endregion
}