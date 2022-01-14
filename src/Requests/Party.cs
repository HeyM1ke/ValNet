using System.Text.Json;
using RestSharp;
using ValNet.Objects.Inventory;
using ValNet.Objects.Party;

namespace ValNet.Requests;

public class Party : RequestBase
{
    public string PartyId { get; set; }

    public Party(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }

    /// <summary>
    ///  Internal Method ran by the riotuser obj, to setup the party class.
    /// </summary>
    internal async void InitialPartySetup()
    {
        var playerInfo = await this.PartyFetchPlayer();
        this.PartyId = playerInfo.CurrentPartyID;
    }

    public async Task<PartyFetchPlayerObj> PartyFetchPlayer()
    {
        var resp = await RiotGlzRequest($"/parties/v1/players/{_user.UserData.sub}", Method.GET);

        if (!resp.isSucc)
            throw new Exception("Failed to get Player Party Information");

        var partyPlayer = JsonSerializer.Deserialize<PartyFetchPlayerObj>(resp.content.ToString());

        return partyPlayer;
    }

    /// <summary>
    /// Creates a new Party : Generates new ID
    /// </summary>
    /// <returns>Returns True if new party is created, Throws an exception if party cannot be created.</returns>
    /// <exception cref="Exception">Throws Exception when new party cannot be created</exception>
    public async Task<bool> CreateNewParty()
    {
        //method creates a new party by removing yourself from the party

        var resp = await RiotGlzRequest($"/parties/v1/players/{_user.UserData.sub}", Method.DELETE);

        if (!resp.isSucc)
            throw new Exception("Failed to Create new party");
        return true;
    }
    
    /// <summary>
    /// Removes player from party from id given
    /// </summary>
    /// <returns>Returns True if player is removed, Throws an exception if an error is hit.</returns>
    /// <exception cref="Exception">Throws Exception whenan error is hit</exception>
    public async Task<bool> RemovePlayerFromParty(string PlayerId)
    {
        var resp = await RiotGlzRequest($"/parties/v1/parties/{this.PartyId}/members/{PlayerId}", Method.DELETE);

        if (!resp.isSucc)
            throw new Exception("Failed to Remove player");
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="PartyId">Checks PartyId Given, If no ID is given operation is ran on current user's party</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<FetchPartyObj> FetchParty(string PartyId = null)
    {
        string pId;
        if (string.IsNullOrEmpty(PartyId))
            pId = this.PartyId;
        else
            pId = PartyId;

        var resp = await RiotGlzRequest($"/parties/v1/parties/{pId}", Method.GET);

        if (!resp.isSucc)
            throw new Exception("Failed to get Party Information");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation; 
    }

    
    
}