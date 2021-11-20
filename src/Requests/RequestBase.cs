using RestSharp;

namespace ValNet.Requests;

public class RequestBase
{

    internal RiotUser _user;

    public RequestBase(RiotUser pUser)
    {
        _user = pUser;
    }
    async Task<DefaultApiResponse> RiotPdRequest(string endpoint, Method method, string extraParams = null)
    {
        IRestRequest pdRequest = new RestRequest($"{_user._riotUrl.pdURL}{endpoint}{extraParams}", method);
        var resp = _user.UserClient.Execute(pdRequest);



        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content
        };

        return response;
    }

    internal async Task<DefaultApiResponse> RiotGlzRequest(string endpoint, Method method, string extraParams = null)
    {
        IRestRequest glzRequest = new RestRequest($"{_user._riotUrl.glzURL}{endpoint}{extraParams}", method);
        var resp = _user.UserClient.Execute(glzRequest);



        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content
        };

        return response;
    }

    internal async Task<DefaultApiResponse> CustomRequest(string url, Method method, string extraParams = null)
    {
        IRestRequest customReq = new RestRequest($"{url}{extraParams}", method);
        var resp = _user.UserClient.Execute(customReq);

        DefaultApiResponse response = new()
        {
            isSucc = resp.IsSuccessful,
            content = resp.Content
        };

        return response;
    }


}