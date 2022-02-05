namespace ValNet.Objects.Player;

public class CompetitiveUpdateObj
{
    public int Version { get; set; }
    public string Subject { get; set; }
    public List<CompetitiveUpdateMatch> Matches { get; set; }
    
    public class CompetitiveUpdateMatch
    {
        public string MatchID { get; set; }
        public string MapID { get; set; }
        public string SeasonID { get; set; }
        public object MatchStartTime { get; set; }
        public int TierAfterUpdate { get; set; }
        public int TierBeforeUpdate { get; set; }
        public int RankedRatingAfterUpdate { get; set; }
        public int RankedRatingBeforeUpdate { get; set; }
        public int RankedRatingEarned { get; set; }
        public int RankedRatingPerformanceBonus { get; set; }
        public string CompetitiveMovement { get; set; }
        public int AFKPenalty { get; set; }
    }
}