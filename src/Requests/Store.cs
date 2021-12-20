using RestSharp;
using System.Text.Json;
using ValNet.Objects.Store;

namespace ValNet.Requests;

public class Store : RequestBase
{
    public Store(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    public async Task<PlayerStore> GetPlayerStore()
    {
        var resp =  await RiotPdRequest($"/store/v2/storefront/{_user.UserData.sub}", Method.GET);
        return JsonSerializer.Deserialize<PlayerStore>(resp.content.ToString());
    }
}