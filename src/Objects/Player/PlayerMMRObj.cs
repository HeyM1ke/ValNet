using System.Text.Json.Serialization;

namespace ValNet.Objects.Player;

public class PlayerMMRObj
{
    public long Version { get; set; }
    public string Subject { get; set; }
    
    [JsonPropertyName("NewPlayerExperienceFinished")]
    public bool bNewPlayerExperienceFinished { get; set; }
    
    [JsonPropertyName("QueueSkills")]
    public QueueSkills QueueData { get; set; }
    public CompetitiveUpdate LatestCompetitiveUpdate { get; set; }
    public bool IsLeaderboardAnonymized { get; set; }
    public bool IsActRankBadgeHidden { get; set; }
    
    
    public class QueueSkills
    {
        public Competitive competitive { get; set; }
        public Custom custom { get; set; }
        public Deathmatch deathmatch { get; set; }
        public Ggteam ggteam { get; set; }
        public Newmap newmap { get; set; }
        public Onefa onefa { get; set; }
        public Seeding seeding { get; set; }
        public Spikerush spikerush { get; set; }
        public Unrated unrated { get; set; }
        
        public class Custom
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Deathmatch
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Ggteam
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Newmap
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Onefa
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Seeding
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class Spikerush
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }
    
    public class Unrated
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }
    
    public class Competitive
    {
        public int TotalGamesNeededForRating { get; set; }
        public int TotalGamesNeededForLeaderboard { get; set; }
        public int CurrentSeasonGamesNeededForRating { get; set; }
        public SeasonalInfoBySeasonID SeasonalInfoBySeasonID { get; set; }
    }

    public class ActInformation
    {
        public string SeasonID { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfWinsWithPlacements { get; set; }
        public int NumberOfGames { get; set; }
        public int Rank { get; set; }
        public int CapstoneWins { get; set; }
        public int LeaderboardRank { get; set; }
        public int CompetitiveTier { get; set; }
        public int RankedRating { get; set; }
        public Dictionary<int,int> WinsByTier { get; set; }
        public int GamesNeededForRating { get; set; }
        public int TotalWinsNeededForRank { get; set; }
    }
    
    
    public class SeasonalInfoBySeasonID
    {
        [JsonPropertyName("0530b9c4-4980-f2ee-df5d-09864cd00542")]
        public ActInformation _0530b9c44980F2eeDf5d09864cd00542 { get; set; }

        [JsonPropertyName("46ea6166-4573-1128-9cea-60a15640059b")]
        public ActInformation _46ea6166457311289cea60a15640059b { get; set; }

        [JsonPropertyName("4cb622e1-4244-6da3-7276-8daaf1c01be2")]
        public ActInformation _4cb622e142446da372768daaf1c01be2 { get; set; }

        [JsonPropertyName("52e9749a-429b-7060-99fe-4595426a0cf7")]
        public ActInformation _52e9749a429b706099fe4595426a0cf7 { get; set; }

        [JsonPropertyName("97b6e739-44cc-ffa7-49ad-398ba502ceb0")]
        public ActInformation _97b6e73944ccFfa749ad398ba502ceb0 { get; set; }

        [JsonPropertyName("a16955a5-4ad0-f761-5e9e-389df1c892fb")]
        public ActInformation A16955a54ad0F7615e9e389df1c892fb { get; set; }

        [JsonPropertyName("ab57ef51-4e59-da91-cc8d-51a5a2b9b8ff")]
        public ActInformation Ab57ef514e59Da91Cc8d51a5a2b9b8ff { get; set; }
    }
    }
}


// Possible reuse in comp updates.
public class CompetitiveUpdate
{
    public string MatchID { get; set; }
    public string MapID { get; set; }
    public string SeasonID { get; set; }
    public long MatchStartTime { get; set; }
    public int TierAfterUpdate { get; set; }
    public int TierBeforeUpdate { get; set; }
    public int RankedRatingAfterUpdate { get; set; }
    public int RankedRatingBeforeUpdate { get; set; }
    public int RankedRatingEarned { get; set; }
    public int RankedRatingPerformanceBonus { get; set; }
    public string CompetitiveMovement { get; set; }
    public int AFKPenalty { get; set; }
}