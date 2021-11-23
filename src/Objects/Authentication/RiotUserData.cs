namespace ValNet.Objects.Authentication;

public class RiotUserData
{
    public string country { get; set; }
    public string sub { get; set; }
    public bool email_verified { get; set; }
    public string player_plocale { get; set; }
    public long country_at { get; set; }
    public PwInfo pw { get; set; }
    public bool phone_number_verified { get; set; }
    public bool account_verified { get; set; }
    public object ppid { get; set; }
    public string player_locale { get; set; }
    public AccountInfo acct { get; set; }
    public int age { get; set; }
    public string jti { get; set; }
}
public class PwInfo
{
    public long cng_at { get; set; }
    public bool reset { get; set; }
    public bool must_reset { get; set; }
}

public class AccountInfo
{
    public int type { get; set; }
    public string state { get; set; }
    public bool adm { get; set; }
    public string game_name { get; set; }
    public string tag_line { get; set; }
    public long created_at { get; set; }
}


