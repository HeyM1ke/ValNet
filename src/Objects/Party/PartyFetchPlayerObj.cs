using System.Text.Json.Serialization;

namespace ValNet.Objects.Party;

public class PartyFetchPlayerObj
{
    public string Subject { get; set; }
    public long Version { get; set; }
    public string CurrentPartyID { get; set; }
    public List<PartyInvite> Invites { get; set; }
    public List<object> Requests { get; set; }
    [JsonPropertyName("PlatformInfo")]
    public PlatformInfo CurrentPlatformInfo { get; set; }
    
    public class PlatformInfo
    {
        public string platformType { get; set; }
        public string platformOS { get; set; }
        public string platformOSVersion { get; set; }
        public string platformChipset { get; set; }
    }
    
    public class PartyInvite
    {
        public string ID { get; set; }
        public string PartyID { get; set; }
        public string Subject { get; set; }
        public string InvitedBySubject { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RefreshedAt { get; set; }
        public long ExpiresIn { get; set; }
    }

}