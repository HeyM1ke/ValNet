namespace ValNet.Objects;

public class RiotSessionObj
{
    public string subject { get; set; }
    public string cxnState { get; set; }
    public string clientID { get; set; }
    public string clientVersion { get; set; }
    public string loopState { get; set; }
    public string loopStateMetadata { get; set; }
    public int version { get; set; }
    public DateTime lastHeartbeatTime { get; set; }
    public DateTime expiredTime { get; set; }
    public int heartbeatIntervalMillis { get; set; }
    public string playtimeNotification { get; set; }
    public int playtimeMinutes { get; set; }
    public bool isRestricted { get; set; }
    public DateTime userinfoValidTime { get; set; }
    public string restrictionType { get; set; }
    public ClientPlatformInfo clientPlatformInfo { get; set; }
    
    public class ClientPlatformInfo
    {
        public string platformType { get; set; }
        public string platformOS { get; set; }
        public string platformOSVersion { get; set; }
        public string platformChipset { get; set; }
    }
}