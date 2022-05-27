namespace ValNet.Requests;

public class Pregame : RequestBase
{
    public Pregame(RiotUser pUser) : base(pUser)
    {
        _user = pUser;
    }
}