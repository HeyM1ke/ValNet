using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using RestSharp;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using WebSocketSharp;

namespace ValNet.Requests;

public class Authentication : RequestBase
{
    public Authentication(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    #region Riot Authentication Urls

    private const string authUrl = "https://auth.riotgames.com/api/v1/authorization";
    private const string entitleUrl = "https://entitlements.auth.riotgames.com/api/token/v1";
    private const string userInfoUrl = "https://auth.riotgames.com/userinfo";
    private const string regionUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/product/valorant";
    private const string xmppPasUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat";
    private const string gameEntitlementUrl =
        "https://clientconfig.rpg.riotgames.com/api/v1/config/player?namespace=keystone.products.valorant.patchlines";

    private const string cookieReauth =
        "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1";
    #endregion

    #region Authentication Objects

    internal Lockfile userLockfile;
    public WebSocket userWebsocket;

    #endregion

    #region Authentication Methods

    public async Task<AuthenticationStatus> AuthenticateWithCloud()
    {
        if (_user.loginData.username == null || _user.loginData.password == null)
            throw new Exception("Username or password are empty, please retry when there are values in place.");

        // get initial cookies for auth
        var initCookies = new RestRequest(authUrl, Method.Post);

        var cookieData = new
        {
            client_id = "play-valorant-web-prod",
            nonce = 1,
            redirect_uri = "https://playvalorant.com/opt_in",
            response_type = "token id_token",
            scope = "account openid"
        };

        initCookies.AddJsonBody(cookieData);
        await _user.UserClient.ExecuteAsync(initCookies);

        // Auth with user details

        var authData = new
        {
            type = "auth",
            _user.loginData.username,
            _user.loginData.password,
            remember = "true"
        };

        var loginRequest = new RestRequest(authUrl, Method.Put);
        loginRequest.AddJsonBody(authData);
        var resp = await _user.UserClient.ExecuteAsync(loginRequest);

        if (!resp.IsSuccessful && resp.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new Exception("Sorry, you are not allowed to login. " + resp.Content);
        }
        
        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");

        var authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj.error is not null) throw new Exception("Error: Username/Password is incorrect");

        // Two Factor Authentication Block

        //Determine if Two Factor is needed
        if (authObj.type.Equals("multifactor"))
            return new AuthenticationStatus
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
        var multifactorRequest = new RestRequest(authUrl, Method.Put);
        var data = new
        {
            type = "multifactor",
            code,
            rememberDevice = true
        };

        multifactorRequest.AddJsonBody(data);
        var resp = await _user.UserClient.ExecuteAsync(multifactorRequest);

        if (!resp.IsSuccessful)
            throw new Exception("An Error has Occurred");

        var authObj = JsonSerializer.Deserialize<AuthorizationJson>(resp.Content);

        if (authObj.error is not null && authObj.error.Equals("multifactor_attempt_failed"))
            return new AuthenticationStatus
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
        GetRiotXmppPasToken();
        _user.AuthType = AuthType.Cloud;

        return new AuthenticationStatus
        {
            bIsAuthComplete = true
        };
    }

    public async void AuthenticateWithToken(string redirectUrl)
    {
        // get initial cookies for auth
        var initCookies = new RestRequest(cookieReauth, Method.Get);

        /*var cookieData = new
        {
            client_id = "play-valorant-web-prod",
            nonce = 1,
            redirect_uri = "https://playvalorant.com/opt_in",
            response_type = "token id_token",
            scope = "account openid"
        };*/
        await _user.UserClient.ExecuteAsync(initCookies);

        ParseWebToken(redirectUrl);
        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");

        _user.tokenData.entitle = await GetEntitlementToken();
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        _user.UserData = await GetUserData();
        _user.UserRegion = await GetUserRegion();
        GetCurrentGameVersion();
        GetRiotXmppPasToken();
        _user.AuthType = AuthType.Cookie;
    }

    public async Task<AuthenticationStatus> AuthenticateWithSocket()
    {
        //Check for Lockfile
        if (ParseLockFile() == false)
            throw new Exception("Game is not Open.");

        //Lockfile needs to exist for the remaining methods need to run.
        ConnectToWebsocket(); // Connects to the websocket

        var sockToken =
            new RestRequest($"https://127.0.0.1:{userLockfile.port}/entitlements/v1/token");

        sockToken.AddHeader("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{_user.Authentication.userLockfile.password}"))}");


        var resp = await _user.SocketClient.ExecuteAsync(sockToken);

        if (!resp.IsSuccessful)
            throw new Exception("Error reaching game.");
        var tokens = JsonSerializer.Deserialize<WebsocketTokens>(resp.Content);

        _user.tokenData.entitle = tokens.token;
        _user.tokenData.access = tokens.accessToken;

        _user.UserClient.AddDefaultHeader("Authorization", $"Bearer {_user.tokenData.access}");
        _user.UserClient.AddDefaultHeader("X-Riot-Entitlements-JWT", _user.tokenData.entitle);

        // Get ID Token for Region
        var idResp = await _user.Requests.WebsocketRequest($"/rso-auth/v2/authorizations/valorant-client", Method.Get);
        
        if (!idResp.isSucc)
            throw new Exception("Error reaching game. | Id Token Req");
        
        var idToken = JsonSerializer.Deserialize<RSOWebsocketObj>(idResp.content.ToString());

        _user.tokenData.idToken = idToken.authorization.idToken.token;
        _user.UserRegion = await GetUserRegion();
        _user.UserData = await GetUserData();
        GetRiotXmppPasToken();
        GetCurrentGameVersion();
        _user.AuthType = AuthType.Socket;

        return new AuthenticationStatus
        {
            bIsAuthComplete = true
        };
    }

    public async Task AuthenticateWithCookies()
    {
        var initCookies = new RestRequest(authUrl, Method.Post);

        var cookieData = new
        {
            client_id = "play-valorant-web-prod",
            nonce = 1,
            redirect_uri = "https://playvalorant.com/opt_in",
            response_type = "token id_token",
            scope = "account openid"
        };

        initCookies.AddJsonBody(cookieData);
        var resp = await _user.UserClient.ExecuteAsync(initCookies);

        if (!resp.IsSuccessful && resp.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new Exception("Sorry, you are not allowed to login. " + resp.Content);
        }
        
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
        GetRiotXmppPasToken();
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

        var nullObj = new
        {
        };

        request.AddJsonBody(nullObj);

        var resp = await _user.UserClient.ExecuteAsync(request);

        if (!resp.IsSuccessful)
            throw new Exception("Failed to get entitlement token.");


        // Testing new Json Parsing 
        var entitlement_token = "";
        var respJson = JsonDocument.Parse(resp.Content);
        if (respJson.RootElement.TryGetProperty("entitlements_token", out var tokenElement))
            entitlement_token = tokenElement.GetString();


        return entitlement_token;
    }

    private async Task<RiotUserData?> GetUserData()
    {
        var userDataReq = new RestRequest(userInfoUrl);
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
            var request = new RestRequest(regionUrl, Method.Put);
            var idTokData = new
            {
                id_token = _user.tokenData.idToken
            };
            request.AddJsonBody(idTokData);
            var resp = await _user.UserClient.ExecuteAsync(request);
            if (!resp.IsSuccessful)
                throw new Exception("Failed to get user region.");

            var respNode = JsonNode.Parse(resp.Content);
            var authObj = respNode["affinities"]["live"];

            liveRegion = authObj.ToString();
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
        }

        return RiotRegion.AP;
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
            .ExecuteAsync(new RestRequest("https://valorant-api.com/v1/version")).Result.Content);

        _user.UserClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
        _user.SocketClient.AddDefaultHeader("X-Riot-ClientVersion", resp.data.riotClientVersion);
        _user.UserClient.AddDefaultHeader("X-Riot-ClientPlatform",
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        _user.SocketClient.AddDefaultHeader("X-Riot-ClientPlatform",
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
    }

    // Switch back to private and change obj
    public async Task<List<PatchlineObj>> GetPlayerGameEntitlements()
    {
        // "keystone.products.valorant.patchlines.pbe"
        var avaliblePatchLines = new List<PatchlineObj>();
        var resp = await CustomRequest(gameEntitlementUrl, Method.Get);

        var doc = JsonDocument.Parse(resp.content.ToString());

        foreach (var entitlement in doc.RootElement.EnumerateObject())
        {
            var split = entitlement.Name.Split('.');
            avaliblePatchLines.Add(new PatchlineObj
            {
                PatchlineName = split[split.Length - 1].ToUpper(),
                PatchlinePath = split[split.Length - 1]
            });
        }

        return avaliblePatchLines;
    }

    public async void GetRiotXmppPasToken()
    {
        var resp = await CustomRequest(xmppPasUrl, Method.Get);
        _user.tokenData.pasToken = resp.content.ToString();
        
    }
    #endregion
}