namespace ValNet.Objects;

public class ThirdParty
{
    
}

public class ValorantApi_VersionResp
{
    public int status { get; set; }
    public Data data { get; set; }
    
    public class Data
    {
        public string manifestId { get; set; }
        public string branch { get; set; }
        public string version { get; set; }
        public string buildVersion { get; set; }
        public string riotClientVersion { get; set; }
        public DateTime buildDate { get; set; }
    }
}