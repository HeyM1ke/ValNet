using RestSharp;

namespace ValNet.Requests;

public class RequestBase
{
    internal RiotUser _user;

    async Task<DefaultApiResponse> RiotPdRequest(string endpoint, Method method, string extraParams = null)
    {
        IRestRequest pdRequest = new RestRequest($"{_user._riotUrl.pdURL}{endpoint}{extraParams}", method);
        
        
        DefaultApiResponse response = new()
        {
            
        };
        return response;
    }
}