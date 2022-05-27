using CliWrap;

using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace ValNet.Objects;

public class CurlResponse<T> : CurlResponse
{
    private readonly JsonSerializerOptions? _jsonOptions;

    public T? Data { get; private set; }

    public CurlResponse(JsonSerializerOptions? jsonOptions = null)
    {
        _jsonOptions = jsonOptions;
    }

    public override void ParseContent()
    {
        try
        {
            Data = JsonSerializer.Deserialize<T>(Content, _jsonOptions);
        }
        catch
        {
            // ignored
        }
    }
}

public class CurlResponse
{
    public Version HttpVersion { get; private set; }
    public HttpStatusCode Status { get; private set; }
    public string StatusDescription { get; private set; }
    public Dictionary<string, string> Headers { get; }
    public Dictionary<string, string> Cookies { get; }
    public string? Content { get; private set; }

    private bool _headers;
    private bool _data;

    public CurlResponse()
    {
        Headers = new Dictionary<string, string>();
        Cookies = new Dictionary<string, string>();
    }

    public PipeTarget GetPipeTarget() => PipeTarget.ToDelegate(Callback);

    public void Callback(string line)
    {
        if (_data)
        {
            if (Content is not null)
            {
                Debugger.Break();
                return;
            }

            Content = line;
            ParseContent();
            return;
        }

        if (_headers)
        {
            if (string.IsNullOrEmpty(line))
            {
                _data = true;
                return;
            }

            var doubleDotIndex = line.IndexOf(':');

            if (line.StartsWith("Set-Cookie", StringComparison.OrdinalIgnoreCase))
            {
                var cookieNameEnd = line.IndexOf('=', doubleDotIndex + 2);
                var cookieName = line[(doubleDotIndex + 2)..cookieNameEnd];
                var cookieValueEnd = line.IndexOf("; ", cookieNameEnd + 1, StringComparison.Ordinal);
                var cookieValue = cookieValueEnd == -1 ? line[(cookieNameEnd + 1)..] : line[(cookieNameEnd + 1)..cookieValueEnd];
                Cookies[cookieName] = cookieValue;
            }
            else
            {
                var headerName = line[..doubleDotIndex];
                var headerValue = line[(doubleDotIndex + 2)..];
                Headers[headerName] = headerValue;
            }

            return;
        }

        var slashIndex = line.IndexOf('/');
        var spaceIndex = line.IndexOf(' ', slashIndex + 1);
        var span = line.AsSpan();
        HttpVersion = Version.Parse(span.Slice(slashIndex + 1, spaceIndex - (slashIndex + 1)));

        var nextSpaceIndex = line.IndexOf(' ', spaceIndex + 1);
        Status = (HttpStatusCode)int.Parse(span.Slice(spaceIndex + 1, nextSpaceIndex - (spaceIndex + 1)));

        StatusDescription = line[(nextSpaceIndex + 1)..];

        _headers = true;
    }

    public virtual void ParseContent() { }
}
