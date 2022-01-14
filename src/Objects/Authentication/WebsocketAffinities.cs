namespace ValNet.Objects.Authentication;

public class WebsocketAffinities
{
    public Affinities affinities { get; set; }
    public int expiry { get; set; }
    public int issuedAt { get; set; }
    public string product { get; set; }
    public string puuid { get; set; }
    public string source { get; set; }
    public string token { get; set; }
    
    public class Affinities
    {
        public string live { get; set; }
        public string pbe { get; set; }
    }
}