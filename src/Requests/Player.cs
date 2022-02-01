using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ValNet.Requests
{
    public class Player : RequestBase
    {
        public Player(RiotUser pUser) : base(pUser)
        {
            _user = pUser;
        }


        // Get Player MMR

        public async Task<object?> GetPlayerMmr()
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Player MMR");

            return JsonSerializer.Deserialize<object>(resp.content.ToString()); 
        }
        // Get Player Competitive Updates

        public async Task<Object?> GetCompetitiveUpdates()
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }
        // Get Player Match History
        public async Task<Object?> GetPlayerMatchHistory()
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }

        public async Task<Object?> GetPlayerMatchHistory(int start, int end)
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }

        // Get Player's Entire History
        public async Task<Object?> GetPlayerEntireMatchHistory(string puuid, string region)
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }
        // Get Other Players History
        public async Task<Object?> GetPlayerMatchHistory(string puuid, string region)
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }

    }
}
