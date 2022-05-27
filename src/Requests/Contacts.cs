using System.Text.Json;
using RestSharp;
using ValNet.Objects.Contacts;

namespace ValNet.Requests;

public class Contracts : RequestBase
{
    private const string currentBattlepassId = "c1cd8895-4bd2-466d-e7ff-b489e3bc3775";

    public Contracts(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    /// <summary>
    /// Gets Current Battlepass 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<ContactsFetchObj.Contract> GetCurrentBattlepass()
    {
        var resp = await RiotPdRequest($"/contracts/v1/contracts/{_user.UserData.sub}", Method.Get);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Battlepass");

        var des = JsonSerializer.Deserialize<ContactsFetchObj>(resp.content.ToString());

        var currentBPContract = des.Contracts.Find(contact => contact.ContractDefinitionID == currentBattlepassId);

        if (currentBPContract != null)
            return currentBPContract;
        throw new Exception("Could not find current BP");
    }
    
    /// <summary>
    /// Gets Contract By ID
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<ContactsFetchObj.Contract> GetContract(string contractId)
    {
        var resp = await RiotPdRequest($"/contracts/v1/contracts/{_user.UserData.sub}", Method.Get);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Contacts");

        var des = JsonSerializer.Deserialize<ContactsFetchObj>(resp.content.ToString());

        var currentBPContract = des.Contracts.Find(contact => contact.ContractDefinitionID == contractId);

        if (currentBPContract != null)
            return currentBPContract;
        throw new Exception("Could not find Contract");
    }
    
    /// <summary>
    /// Returns all User Contracts
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<ContactsFetchObj> GetAllContracts()
    {
        var resp = await RiotPdRequest($"/contracts/v1/contracts/{_user.UserData.sub}", Method.Get);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Contacts");

        var data = JsonSerializer.Deserialize<ContactsFetchObj>(resp.content.ToString());

        if (data != null)
            return data;
        throw new Exception("Could not find Contract");
    }
}