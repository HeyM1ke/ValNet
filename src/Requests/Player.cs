using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ValNet.Objects.Player;

namespace ValNet.Requests
{
    public class Player : RequestBase
    {
        public Player(RiotUser pUser) : base(pUser)
        {
            _user = pUser;
        }


        // Get Player MMR

        public async Task<PlayerMMRObj?> GetPlayerMmr()
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Player MMR");

            return JsonSerializer.Deserialize<PlayerMMRObj>(resp.content.ToString()); 
        }
        
        /// <summary>
        /// Get Player Competitive Updates
        /// </summary>
        /// <returns>CompetitiveUpdateObj</returns>
        /// <exception cref="Exception"></exception>
        public async Task<CompetitiveUpdateObj?> GetCompetitiveUpdates()
        {
            var resp = await RiotPdRequest($"/mmr/v1/players/{_user.UserData.sub}/competitiveupdates", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<CompetitiveUpdateObj>(resp.content.ToString());
        }
        
        /// <summary>
        /// Get Player's Match History
        /// </summary>
        /// <returns>MatchHistoryObj With Data</returns>
        /// <exception cref="Exception"></exception>
        public async Task<MatchHistoryObj?> GetPlayerMatchHistory()
        {
            var resp = await RiotPdRequest($"/match-history/v1/history/{_user.UserData.sub}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Match History");

            return JsonSerializer.Deserialize<MatchHistoryObj>(resp.content.ToString());
        }
        /// <summary>
        /// Get Player's Match History
        /// </summary>
        /// <param name="start">Starting Index for matches to look for</param>
        /// <param name="end">Ending Index for matches to look for</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Object?> GetPlayerMatchHistory(int start, int end)
        {
            var resp = await RiotPdRequest($"/match-history/v1/history/{_user.UserData.sub}?startIndex={start}&endIndex={end}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Match History");

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
            if (string.IsNullOrEmpty(puuid))
                throw new Exception("Player id is empty/null");
            
            var resp = await RiotPdRequest($"/match-history/v1/history/{puuid}", Method.Get);

            if (!resp.isSucc)
                throw new Exception("Failed to get Competitive Updates");

            return JsonSerializer.Deserialize<object>(resp.content.ToString());
        }

    }
}
