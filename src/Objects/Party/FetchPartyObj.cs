using System.Text.Json.Serialization;

namespace ValNet.Objects.Party;

public class FetchPartyObj
{
    // Should Work, Test in a bit
    
    public string ID { get; set; }
    public string MUCName { get; set; }
    public string VoiceRoomID { get; set; }
    public long Version { get; set; }
    public string ClientVersion { get; set; }
    public List<Member> Members { get; set; }
    public string State { get; set; }
    public string PreviousState { get; set; }
    public string StateTransitionReason { get; set; }
    public string Accessibility { get; set; }
    [JsonPropertyName("CustomGameData")]
    public CustomGameData PartyCustomGameData { get; set; }
    [JsonPropertyName("MatchmakingData")]
    public MatchmakingData PartyMatchmakingData { get; set; }
    public object Invites { get; set; }
    public List<object> Requests { get; set; }
    public DateTime QueueEntryTime { get; set; }
    [JsonPropertyName("ErrorNotification")]
    public ErrorNotification PartyErrorNotification { get; set; }
    public int RestrictedSeconds { get; set; }
    public List<string> EligibleQueues { get; set; }
    public List<object> QueueIneligibilities { get; set; }
    [JsonPropertyName("CheatData")]
    public CheatData PartyCheatData { get; set; }
    public List<object> XPBonuses { get; set; }
    
    
    public class CheatData
    {
        public string GamePodOverride { get; set; }
        public bool ForcePostGameProcessing { get; set; }
    }
    
    public class PlayerIdentity
    {
        public string Subject { get; set; }
        public string PlayerCardID { get; set; }
        public string PlayerTitleID { get; set; }
        public int AccountLevel { get; set; }
        public string PreferredLevelBorderID { get; set; }
        public bool Incognito { get; set; }
        public bool HideAccountLevel { get; set; }
    }

    /// <summary>
    /// Gives information regarding ping to different servers to for player assigned to.
    /// </summary>
    public class Pings
    {
        public int Ping { get; set; }
        public string GamePodID { get; set; }
    }

    /// <summary>
    /// Information regarding a player in the party
    /// </summary>
    public class Member
    {
        public string Subject { get; set; }
        public int CompetitiveTier { get; set; }
        public PlayerIdentity PlayerIdentity { get; set; }
        public object SeasonalBadgeInfo { get; set; }
        public bool IsOwner { get; set; }
        public int QueueEligibleRemainingAccountLevels { get; set; }
        public List<Pings> Pings { get; set; }
        public bool IsReady { get; set; }
        public bool IsModerator { get; set; }
        public bool UseBroadcastHUD { get; set; }
        public string PlatformType { get; set; }
    }

    /// <summary>
    /// Information regarding current party's custom game's settings
    /// </summary>
    public class Settings
    {
        public string Map { get; set; }
        public string Mode { get; set; }
        public bool UseBots { get; set; }
        public string GamePod { get; set; }
        public object GameRules { get; set; }
    }

    /// <summary>
    /// Information regarding current party's custom game team information
    /// </summary>
    public class Membership
    {
        public List<CustomgameTeamMember> teamOne { get; set; }
        public List<CustomgameTeamMember> teamTwo { get; set; }
        public List<CustomgameTeamMember> teamSpectate { get; set; }
        public List<CustomgameTeamMember> teamOneCoaches { get; set; }
        public List<CustomgameTeamMember> teamTwoCoaches { get; set; }
        public class CustomgameTeamMember
        {
            public string Subject { get; set; }
        }
    }

    
    /// <summary>
    /// Information regarding current party's custom game information
    /// </summary>
    public class CustomGameData
    {
        public Settings Settings { get; set; }
        public Membership Membership { get; set; }
        public int MaxPartySize { get; set; }
        public bool AutobalanceEnabled { get; set; }
        public int AutobalanceMinPlayers { get; set; }
    }

    /// <summary>
    /// Information regarding current MatchmakingData for party.
    /// </summary>
    public class MatchmakingData
    {
        public string QueueID { get; set; }
        public List<object> PreferredGamePods { get; set; }
        public int SkillDisparityRRPenalty { get; set; }
    }

    // Test later when shooting range is back online
    // Shooting range is offline as of 1/13, test and fix when it comes back to see errors and fix ErorredPlayers Obj
    public class ErrorNotification
    {
        public string ErrorType { get; set; }
        public object ErroredPlayers { get; set; }
    }
}