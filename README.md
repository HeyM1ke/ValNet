# ValNet
A C# Based Valorant library that allows you to interact with data from VALORANT.

# NOTICE
This library is not complete and still has a ways to go and can be improved in many aspects. Please contribute if you would like.

# Dependencies
> This project is built on top of excisting nuget packages. Valnet will become a nuget package when it is complete.

- Restsharp (106.15.0)
- WebSocketSharp


# Authentication Example

> Simple Console Application Example for Authentication

```csharp
using ValNet;
using ValNet.Objects.Authentication;

/*
 * Example Authentication Using ValNet Lib
 * By: Mike
 * Written On 1/20/21
 */

RiotLoginData LoginData = new() // Basic Login Object used to pass authentication data to riot servers.
{
    username = "Username",
    password = "Password"
};

// Valnet's User Class, Can be used multiple times for each riot user you want to actively login to.
// Class Houses every aspect of Valnet and its features.
// There are multiple ways to construct the user.
// You can have it empty or pass in the LoginData on its Instantiation
// You can also pass the region for the account *BUT* region is automatically gotten when authenticating.
// The region is there for future cases just as backup.
RiotUser User;

User = new RiotUser(LoginData); // Instantiate User with their login information

var ResponseData =
    User.Authentication
        .AuthenticateWithCloud(); // Access the authentication class and login with cloud to use login data. Store the Response.

// This will send the login data to Riot servers and setup the User, You can check if everything went well if
// 1. You do not get any Exception thrown to you
// 2. By Checking the ResponseData.bIsAuthComplete Variable to check the status. If True you are good to go.

// As of 1/19/21: Riot has released Multifactor authentication. 
// To see if the user needs to send a two factor authentication code, please check the response data.
// If ResponseData.type is "multifactor", this means there is a multifactor code required.
// To get the data from the multifactor prompt check the ResponseData.multifactorData object.
// This object contains all the data needed for two factor.

if (ResponseData.bIsAuthComplete == false)
{
    if (ResponseData.type.Equals("multifactor")) // Checking if multifactor is needed.
        MultifactorPrompt(); // Method that will continuously prompt for the second code.
}

Console.WriteLine("Login Sucessful!");


void MultifactorPrompt()
{
    var Email = ResponseData.multifactorData
        .email; // Stores the email that riot provides which is the location the code is sent to.

    Console.WriteLine(
        $"Please check {Email} and please type in your {ResponseData.multifactorData.multiFactorCodeLength} digit code. ");
    var code = Console.ReadLine(); // Console Waits for a response

    ResponseData = User.Authentication.AuthenticateTwoFactorCode(code); // Sends the code to riot servers.

    if (ResponseData.error is not null &&
        ResponseData.error.Equals(
            "multifactor_attempt_failed")) // Checks if an error occurs, and if the error is showing that the wrong code was entered.
    {
        Console.WriteLine("The Code is wrong.");
        MultifactorPrompt();
    }
}


```

# Docs Later..
