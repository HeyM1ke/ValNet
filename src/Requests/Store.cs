using RestSharp;
using System.Text.Json;
using ValNet.Objects.Store;

namespace ValNet.Requests;

public class Store : RequestBase
{
    public PlayerStore PlayerStore { get; set; }

    public Store(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    public async Task<PlayerStore> GetPlayerStore()
    {
        var resp =  await RiotPdRequest($"/store/v2/storefront/{_user.UserData.sub}", Method.GET);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Store");

        PlayerStore = JsonSerializer.Deserialize<PlayerStore>(resp.content.ToString());

        return PlayerStore;
    }

    public async Task<object> GetStoreOffers()
    {
        var resp =  await RiotPdRequest("/store/v1/offers/", Method.GET);

        if (!resp.isSucc)
            throw new Exception("Failed to get Store Offers");

        return resp.content;
    }
}