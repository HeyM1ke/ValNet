using System.Text;
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
        var pdRequest = new RestRequest($"{_user._riotUrl.pdURL}{endpoint}{extraParams}", method);
        var resp = await _user.UserClient.ExecuteAsync(pdRequest);

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
        var glzRequest = new RestRequest($"{_user._riotUrl.glzURL}{endpoint}{extraParams}", method);
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
        var customReq = new RestRequest($"{url}{extraParams}", method);
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
        var socketReq =
            new RestRequest($"https://127.0.0.1:{_user.Authentication.userLockfile.port}{endpoint}{extraParams}",
                method);
        socketReq.AddHeader("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{_user.Authentication.userLockfile.password}"))}");
        var data = JsonSerializer.Serialize(body);
        socketReq.AddJsonBody(data);
        var resp = await _user.SocketClient.ExecuteAsync(socketReq);

        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content,
            StatusCode = (int) resp.StatusCode
        };

        return response;
    }
}