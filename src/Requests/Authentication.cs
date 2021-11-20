using ValNet.Objects.Authentication;
using RestSharp;
namespace ValNet.Requests;

public class Authentication : RequestBase 
{

    private const string authUrl = "https://auth.riotgames.com/api/v1/authorization";
    private const string entitleUrl = "https://entitlements.auth.riotgames.com/api/token/v1";
    private const string userInfoUrl = "https://auth.riotgames.com/userinfo";
    private const string cookieJson = "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";"
    public Authentication(RiotUser pUser)
    {
        _user = pUser;
    }
    
    async void AuthenticateWithCloud()
    {
        if (_user.loginData.username == null || _user.loginData.password == null)
            throw new Exception("Username or password are empty, please retry when there are values in place.");

        // get initial cookies for auth
        IRestRequest initCookies = new RestRequest(authUrl);
        initCookies.AddJsonBody(cookieJson);
        _user.UserClient.Execute(initCookies, Method.PUT);

        // Auth with user details

        var authData = new
        {
            type = "auth",
            username = _user.loginData.username,
            password = _user.loginData.password,
            remember = true
        };

        IRestRequest tokenRequest = new RestRequest(entitleUrl);
        tokenRequest.AddJsonBody(authData);
        var resp = _user.UserClient.Execute(tokenRequest, Method.POST);

        if (!resp.IsSuccessful)
            throw new Exception("Failed Login, please check credentials and try again.");



        
        



        _user.AuthType = AuthType.Cloud;
    }
    
    async void AuthenticateWithSocket()
    {
        _user.AuthType = AuthType.Socket;
    }
    
    async void AuthenticateWithCookies()
    {
        
        
        _user.AuthType = AuthType.Cookie;
    }

    void ParseWebToken()
    {
        
    }
}