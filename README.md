# Ternwire .NET SDK

C# client for the Ternwire transactional email API, targeting .NET 8+.
Uses HttpClient and System.Text.Json without third-party dependencies.

```sh
dotnet add package Ternwire
```

## Usage

```csharp
using System.Text.Json.Nodes;
using Ternwire;

using var client = new TernwireClient(apiKey);
var message = await client.SubmitEmailAsync(new JsonObject
{
    ["from"] = "notifications@your-verified-domain.com",
    ["to"] = new JsonArray(JsonValue.Create("recipient@example.com")),
    ["subject"] = "Your receipt",
    ["text"] = "Thanks for your order."
}, "order-123-receipt");
```

The client sends requests to `TernwireClient.DefaultBaseUrl`, `https://ternwire.com`.
To use a local or staging API, pass its origin without `/v1`, and optionally a
timeout (default 30 seconds):

```csharp
using var client = new TernwireClient(apiKey, "http://localhost:3001", TimeSpan.FromSeconds(10));
```

The origin must use https unless it points at localhost or a loopback address.

All 17 operations have PascalCase names ending in `Async`. Inputs and responses
are `JsonObject` values with the API's camelCase fields. Template deletion returns
`null`. Every operation accepts an optional final `CancellationToken`. Reuse a
client for its application's lifetime, then dispose it.
`ListEmailTemplatesAsync(cursor: null, limit: null)` returns one page of summaries;
pass the previous `page.nextCursor` as `cursor` to fetch the next page. Pass a
cancellation token to it by name, as in `cancellationToken: token`.

Catch `TernwireApiException` to inspect `Status`, `Body`, `RawBody`, and `Headers`.
HttpClient failures and cancellation exceptions propagate separately.

Use `Webhooks.Verify(signatureHeader, rawBodyBytes, signingSecret)` before parsing
the request body. The optional fourth argument is tolerance seconds, default 300.

Set `TERNWIRE_API_KEY`, `TERNWIRE_FROM`, and `TERNWIRE_TO` before deliberately
running `dotnet run --project examples/Send`. It submits an email. Set
`TERNWIRE_API_ORIGIN` only to send to a local or staging API instead of production.

Read the full documentation at [ternwire.com/docs](https://ternwire.com/docs), or
contact support@ternwire.com.
Released under the MIT License.
