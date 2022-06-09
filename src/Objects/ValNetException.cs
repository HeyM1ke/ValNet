using System.Net;

namespace ValNet.Objects;

/// <summary>
/// ValNet exception.
/// </summary>
public class ValNetException : Exception
{
    public readonly string RequestContent;
    public readonly HttpStatusCode RequestStatusCode;

    /// <summary>
    ///  Initializes a new instance of <see cref="ValNetException"/> class.
    /// </summary>
    /// <param name="message">Message of the Exception</param>
    public ValNetException(string message, HttpStatusCode code, string requestContent) : base(message)
    {
        RequestStatusCode = code;
        RequestContent = requestContent;
    }
}
