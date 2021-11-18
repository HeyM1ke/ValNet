using ValNet.Objects.Authentication;
namespace ValNet.Requests;

public class Authentication : RequestBase 
{
    public Authentication(RiotUser pUser)
    {
        _user = pUser;
    }
    
    async void AuthenticateWithCloud()
    {
        _user.AuthType = AuthType.Cloud;
    }
    
    async void AuthenticateWithSocket()
    {
        _user.AuthType = AuthType.Socket;
    }
    
    async void AuthenticateWithCookies()
    {
        
        
        _user.AuthType = AuthType.Cookie;
    }

    void ParseWebToken()
    {
        
    }
}