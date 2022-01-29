using System.Text.Json;
using RestSharp;

namespace ValNet.Requests;

public class RequestBase
{
    internal RiotUser _user;

    public RequestBase(RiotUser pUser)
    {
        _user = pUser;
    }

    internal async Task<DefaultApiResponse> RiotPdRequest(string endpoint, Method method, string extraParams = null,
        object body = null)
    {
        RestRequest pdRequest = new RestRequest($"{_user._riotUrl.pdURL}{endpoint}{extraParams}", method);
        var resp = _user.UserClient.ExecuteAsync(pdRequest).Result;

        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content,
            StatusCode = (int) resp.StatusCode
        };

        return response;
    }

    internal async Task<DefaultApiResponse> RiotGlzRequest(string endpoint, Method method, string extraParams = null,
        object body = null)
    {
        RestRequest glzRequest = new RestRequest($"{_user._riotUrl.glzURL}{endpoint}{extraParams}", method);
        var resp = _user.UserClient.ExecuteAsync(glzRequest).Result;


        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content,
            StatusCode = (int) resp.StatusCode
        };

        return response;
    }

    internal async Task<DefaultApiResponse> CustomRequest(string url, Method method, string extraParams = null,
        object body = null)
    {
        RestRequest customReq = new RestRequest($"{url}{extraParams}", method);
        var resp = _user.UserClient.ExecuteAsync(customReq).Result;

        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content,
            StatusCode = (int) resp.StatusCode
        };

        return response;
    }


    public async Task<DefaultApiResponse> WebsocketRequest(string endpoint, Method method, string extraParams = null,
        object body = null)
    {
        RestRequest socketReq =
            new RestRequest($"https://127.0.0.1:{_user.Authentication.userLockfile.port}{endpoint}{extraParams}",
                method);
        socketReq.AddHeader("Authorization",
            $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{_user.Authentication.userLockfile.password}"))}");
        socketReq.AddHeader("X-Riot-ClientPlatform",
            "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
        socketReq.AddHeader("X-Riot-ClientVersion", "release-04.00-shipping-20-655657");
        var data = JsonSerializer.Serialize(body);
        socketReq.AddJsonBody(data);
        var resp = _user.SocketClient.ExecuteAsync(socketReq).Result;

        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content,
            StatusCode = (int) resp.StatusCode
        };

        return response;
    }
}