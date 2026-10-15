using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ternwire;

public sealed class TernwireApiException : Exception
{
    public int Status { get; }
    public string RawBody { get; }
    public JsonNode? Body { get; }
    public IReadOnlyDictionary<string, string[]> Headers { get; }

    public TernwireApiException(int status, string rawBody, IReadOnlyDictionary<string, string[]> headers)
        : base($"Ternwire API returned HTTP {status}")
    {
        Status = status;
        RawBody = rawBody;
        Headers = headers;
        try { Body = JsonNode.Parse(rawBody); }
        catch (JsonException) { Body = null; }
    }
}
