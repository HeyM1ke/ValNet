using System.Text.Json.Serialization;

namespace ValNet.Objects.Contacts;

public class ContactsFetchObj
{
    public int Version { get; set; }
    public string Subject { get; set; }
    public List<Contract> Contracts { get; set; }
    public List<ProcessedMatch> ProcessedMatches { get; set; }
    public string ActiveSpecialContract { get; set; }
    public List<Mission> Missions { get; set; }
    [JsonPropertyName("MissionMetadata")]
    public MissionMetadata MissionsMetadata { get; set; }
    
    
    
    public class ContractProgression
    {
        public int TotalProgressionEarned { get; set; }
        
        public Dictionary<string, int> HighestRewardedLevel { get; set; }
        
        
    }
    public class ProcessedMatch
    {
        public string ID { get; set; }
        public object StartTime { get; set; }
        public object XPGrants { get; set; }
        public object RewardGrants { get; set; }
        public object MissionDeltas { get; set; }
        public object ContractDeltas { get; set; }
        public bool CouldProgressMissions { get; set; }
    }
    public class Contract
    {
        public string ContractDefinitionID { get; set; }
        public ContractProgression ContractProgression { get; set; }
        public int ProgressionLevelReached { get; set; }
        public int ProgressionTowardsNextLevel { get; set; }
    }
    
    public class Mission
    {
        public string ID { get; set; }
        public Dictionary<string, int> Objectives { get; set; }
        public bool Complete { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    public class MissionMetadata
    {
        public bool NPECompleted { get; set; }
        public DateTime WeeklyCheckpoint { get; set; }
        public DateTime WeeklyRefillTime { get; set; }
    }
}