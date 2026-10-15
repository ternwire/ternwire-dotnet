using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Ternwire;

public sealed class TernwireClient : IDisposable
{
    private readonly HttpClient http;
    private readonly string apiKey;
    private readonly string baseUrl;

    public TernwireClient(string apiKey, string baseUrl, TimeSpan? timeout = null)
    {
        if (!Regex.IsMatch(apiKey, @"\Aem_(live|test)_[A-Za-z0-9_-]+\z"))
            throw new ArgumentException("Use an em_live_ or em_test_ API key", nameof(apiKey));
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var origin)
            || (origin.Scheme != "http" && origin.Scheme != "https") || origin.Host.Length == 0
            || origin.UserInfo.Length > 0 || origin.AbsolutePath != "/"
            || origin.Query.Length > 0 || origin.Fragment.Length > 0)
            throw new ArgumentException("baseUrl must be an HTTP(S) origin without /v1", nameof(baseUrl));
        var loopback = IsLoopbackHost(origin);
        if (origin.Scheme == "http" && !loopback)
            throw new ArgumentException("baseUrl must use https unless it points at localhost or a loopback address", nameof(baseUrl));
        var requestTimeout = timeout ?? TimeSpan.FromSeconds(30);
        if (requestTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout));
        this.apiKey = apiKey;
        this.baseUrl = baseUrl.TrimEnd('/');
        // A proxy cannot reach this machine's loopback, and would receive the API key.
        http = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, UseProxy = !loopback }) { Timeout = requestTimeout };
    }

    // Uri has already lowercased the host and bracketed IPv6 literals.
    private static bool IsLoopbackHost(Uri origin) =>
        origin.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6
            ? IPAddress.TryParse(origin.Host.Trim('[', ']'), out var address) && IPAddress.IsLoopback(address)
            : origin.Host == "localhost" || origin.Host.EndsWith(".localhost", StringComparison.Ordinal);

    private static string Id(string value)
    {
        if (!Regex.IsMatch(value, @"\A[a-z0-9][a-z0-9_-]{7,127}\z"))
            throw new ArgumentException("Invalid Ternwire resource ID", nameof(value));
        return value;
    }

    private static string Query(params (string Name, object? Value)[] parameters)
    {
        var present = parameters.Where(parameter => parameter.Value is not null)
            .Select(parameter => parameter.Name + "=" + Uri.EscapeDataString(
                Convert.ToString(parameter.Value, CultureInfo.InvariantCulture)!));
        var query = string.Join("&", present);
        return query.Length == 0 ? "" : "?" + query;
    }

    // BEGIN GENERATED OPERATIONS
    public Task<JsonObject?> SubmitEmailAsync(JsonObject input, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(idempotencyKey);
        return RequestAsync(HttpMethod.Post, "/v1/emails", 202, input, idempotencyKey, cancellationToken);
    }

    public Task<JsonObject?> GetMessageAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/emails/" + Id(messageId), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> CreateEmailTemplateAsync(JsonObject input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RequestAsync(HttpMethod.Post, "/v1/templates", 201, input, null, cancellationToken);
    }

    public Task<JsonObject?> ListEmailTemplatesAsync(string? cursor = null, int? limit = null, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/templates" + Query(("cursor", cursor), ("limit", limit)), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> GetEmailTemplateAsync(string templateId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/templates/" + Id(templateId), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> ReplaceEmailTemplateAsync(string templateId, JsonObject input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RequestAsync(HttpMethod.Put, "/v1/templates/" + Id(templateId), 200, input, null, cancellationToken);
    }

    public Task<JsonObject?> DeleteEmailTemplateAsync(string templateId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Delete, "/v1/templates/" + Id(templateId), 204, null, null, cancellationToken);
    }

    public Task<JsonObject?> CreateAttachmentUploadSlotAsync(JsonObject input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RequestAsync(HttpMethod.Post, "/v1/attachments", 201, input, null, cancellationToken);
    }

    public Task<JsonObject?> GetAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/attachments/" + Id(attachmentId), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> CompleteAttachmentUploadAsync(string attachmentId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Post, "/v1/attachments/" + Id(attachmentId) + "/complete", 202, null, null, cancellationToken);
    }

    public Task<JsonObject?> CreateSendingDomainAsync(JsonObject input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RequestAsync(HttpMethod.Post, "/v1/domains", 201, input, null, cancellationToken);
    }

    public Task<JsonObject?> GetSendingDomainAsync(string domainId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/domains/" + Id(domainId), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> DeleteSendingDomainAsync(string domainId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Delete, "/v1/domains/" + Id(domainId), 202, null, null, cancellationToken);
    }

    public Task<JsonObject?> CreateWebhookEndpointAsync(JsonObject input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RequestAsync(HttpMethod.Post, "/v1/webhook-endpoints", 201, input, null, cancellationToken);
    }

    public Task<JsonObject?> RotateWebhookEndpointSigningSecretAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Post, "/v1/webhook-endpoints/" + Id(endpointId) + "/rotate-signing-secret", 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> GetWebhookAttemptAsync(string attemptId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Get, "/v1/webhook-attempts/" + Id(attemptId), 200, null, null, cancellationToken);
    }

    public Task<JsonObject?> ReplayWebhookAttemptAsync(string attemptId, CancellationToken cancellationToken = default)
    {
        return RequestAsync(HttpMethod.Post, "/v1/webhook-attempts/" + Id(attemptId) + "/replay", 202, null, null, cancellationToken);
    }
    // END GENERATED OPERATIONS

    private async Task<JsonObject?> RequestAsync(HttpMethod method, string path, int expectedStatus,
        JsonObject? body, string? idempotencyKey, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, baseUrl + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (idempotencyKey is not null)
        {
            if (!Regex.IsMatch(idempotencyKey, @"\A[\x21-\x7e]{1,128}\z"))
                throw new ArgumentException("idempotencyKey must contain 1-128 visible ASCII characters", nameof(idempotencyKey));
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        }
        if (body is not null)
            request.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        using var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var rawBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if ((int)response.StatusCode != expectedStatus)
        {
            var headers = response.Headers.Concat(response.Content.Headers)
                .ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
            throw new TernwireApiException((int)response.StatusCode, rawBody, headers);
        }
        if (expectedStatus == 204) return null;
        return JsonNode.Parse(rawBody) as JsonObject
            ?? throw new JsonException("Ternwire API returned a non-object JSON response");
    }

    public void Dispose() => http.Dispose();
}
