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
        var playerInfo = await PartyFetchPlayer();
        PartyId = playerInfo.CurrentPartyID;
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
    /// <exception cref="Exception">Throws Exception when an error is hit</exception>
    public async Task<bool> RemovePlayerFromParty(string PlayerId)
    {
        var resp = await RiotGlzRequest($"/parties/v1/parties/{PartyId}/members/{PlayerId}", Method.DELETE);

        if (!resp.isSucc)
            throw new Exception("Failed to Remove player");
        return true;
    }

    /// <summary>
    /// Gets Party Information
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

    /// <summary>
    /// Sets Current User's Ready Status in the party.
    /// </summary>
    /// <param name="ReadyState">Value set as ready or not ready</param>
    /// <returns>Default Party Obj with basic party information.</returns>
    /// <exception cref="Exception">Throws Exception when an error is hit</exception>
    public async Task<FetchPartyObj> SetReadyStatus(bool ReadyState)
    {
        var data = new
        {
            ready = ReadyState
        };

        var resp = await RiotGlzRequest($"/parties/v1/parties/{PartyId}/members/{_user.UserData.sub}/setReady",
            Method.POST, null, data);

        if (!resp.isSucc)
            throw new Exception("Failed to set Ready Status");

        var PartyResp = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyResp;
    }

    /// <summary>
    /// Refreshes Competitive Tier
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">An error thrown when Competitive Tier cannot be refreshed.</exception>
    public async Task<FetchPartyObj> RefreshCompetitiveTier()
    {
        var resp = await RiotGlzRequest(
            $"/parties/v1/parties/{PartyId}/members/{_user.UserData.sub}/refreshCompetitiveTier", Method.POST);

        if (!resp.isSucc)
            throw new Exception("Failed to Refresh Competitive Tier");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation;
    }

    /// <summary>
    /// Refreshes Player Identitys
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Error thrown when Player Identitys fails.</exception>
    public async Task<FetchPartyObj> RefreshPlayerIdentity()
    {
        var resp = await RiotGlzRequest(
            $"/parties/v1/parties/{PartyId}/members/{_user.UserData.sub}/refreshPlayerIdentity", Method.POST);

        if (!resp.isSucc)
            throw new Exception("Failed to Refresh Player Identity");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation;
    }

    /// <summary>
    /// Refreshs Party Pings
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Error thrown when Party Pings fails.</exception>
    public async Task<FetchPartyObj> RefreshPings()
    {
        var resp = await RiotGlzRequest($"/parties/v1/parties/{PartyId}/members/{_user.UserData.sub}/refreshPings",
            Method.POST);

        if (!resp.isSucc)
            throw new Exception("Failed to Refresh Refresh Pings");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation;
    }

    /// <summary>
    /// Changes party queue to specified mode.
    /// </summary>
    /// <param name="QueueId">ID for Gamemode to swap party to.</param>
    /// <returns></returns>
    /// <exception cref="Exception">Error thrown when queue change fails.</exception>
    public async Task<FetchPartyObj> ChangeQueue(string QueueId)
    {
        var data = new
        {
            queueId = QueueId
        };

        var resp = await RiotGlzRequest($"/parties/v1/parties/{PartyId}/queue", Method.POST, null, data);

        if (resp.StatusCode == 403)
            throw new Exception("Forbidden : Queue Selected is Disabled");

        if (!resp.isSucc)
            throw new Exception("Failed to Refresh Refresh Pings");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation;
    }

    /// <summary>
    /// Changes party queue to specified mode.
    /// </summary>
    /// <param name="Queue">ValorantQueueEnum Value, passed in</param>
    /// <returns></returns>
    /// <exception cref="Exception">Error thrown when queue change fails.</exception>
    public async Task<FetchPartyObj> ChangeQueue(ValorantQueueEnum Queue)
    {
        var data = new
        {
            queueId = Queue.ToString()
        };

        var resp = await RiotGlzRequest($"/parties/v1/parties/{PartyId}/queue", Method.POST, null, data);

        if (resp.StatusCode == 403)
            throw new Exception("Forbidden : Queue Selected is Disabled");

        if (!resp.isSucc)
            throw new Exception("Failed to Refresh Refresh Pings");

        var PartyInformation = JsonSerializer.Deserialize<FetchPartyObj>(resp.content.ToString());

        return PartyInformation;
    }
}