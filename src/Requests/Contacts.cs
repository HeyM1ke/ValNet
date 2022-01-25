using System.Text.Json;
using RestSharp;
using ValNet.Objects.Contacts;

namespace ValNet.Requests;

public class Contracts : RequestBase
{
    private const string currentBattlepassId = "60f2e13a-4834-0a18-5f7b-02b1a97b7adb";

    public Contracts(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    public async Task<ContactsFetchObj.Contract> GetCurrentBattlepass()
    {
        var resp = await RiotPdRequest($"/contracts/v1/contracts/{_user.UserData.sub}", Method.GET);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Store");

        var des = JsonSerializer.Deserialize<ContactsFetchObj>(resp.content.ToString());

        var currentBPContract = des.Contracts.Find(contact => contact.ContractDefinitionID == currentBattlepassId);

        if (currentBPContract is not null)
            return currentBPContract;
        else
            throw new Exception("Could not find current BP");
    }
}