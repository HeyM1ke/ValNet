using System.Text.Json;
using RestSharp;
using ValNet.Objects.Inventory;

namespace ValNet.Requests;

public class Inventory : RequestBase
{
    public PlayerInventory CurrentInventory { get; set; }

    public Inventory(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    public async Task<PlayerInventory> GetPlayerInventory()
    {
        var resp = await RiotPdRequest($"/personalization/v2/players/{_user.UserData.sub}/playerloadout", Method.Get);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Store");

        CurrentInventory = JsonSerializer.Deserialize<PlayerInventory>(resp.content.ToString());

        return CurrentInventory;
    }

    public async Task<bool> SetPlayerInventory(PlayerInventory inventory)
    {
        var jsonData = JsonSerializer.Serialize(inventory);

        var resp = await RiotPdRequest($"/personalization/v2/players/{_user.UserData.sub}/playerloadout", Method.Put,
            null, jsonData);

        if (!resp.isSucc) return false;

        CurrentInventory = JsonSerializer.Deserialize<PlayerInventory>(resp.content.ToString());
        return true;
    }
}