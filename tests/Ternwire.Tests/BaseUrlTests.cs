using System.Net;
using Ternwire;
using Xunit;

public class BaseUrlTests
{
    private const string ApiKey = "em_test_abc123";
    private const string MessageId = "msg_12345678";

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
        }
    }

    private static async Task<Uri> RequestUriAsync(string? baseUrl)
    {
        var handler = new RecordingHandler();
        using var client = new TernwireClient(ApiKey, baseUrl, timeout: null, handler);
        await client.GetMessageAsync(MessageId);
        return Assert.Single(handler.Requests);
    }

    [Fact]
    public void DefaultBaseUrlIsTheProductionOrigin()
    {
        Assert.Equal("https://ternwire.com", TernwireClient.DefaultBaseUrl);
    }

    [Fact]
    public async Task OmittedBaseUrlSendsToTheProductionApi()
    {
        Assert.Equal("https://ternwire.com/v1/emails/" + MessageId, (await RequestUriAsync(null)).AbsoluteUri);
    }

    [Fact]
    public async Task ExplicitBaseUrlWinsOverTheDefault()
    {
        Assert.Equal("http://localhost:3001/v1/emails/" + MessageId, (await RequestUriAsync("http://localhost:3001")).AbsoluteUri);
    }

    [Fact]
    public void ApiKeyAloneConstructsAClient()
    {
        using var client = new TernwireClient(ApiKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("http://ternwire.com")]
    [InlineData("https://ternwire.com/v1")]
    public void InvalidExplicitBaseUrlIsRejected(string baseUrl)
    {
        Assert.Throws<ArgumentException>(() => new TernwireClient(ApiKey, baseUrl));
    }
}
