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

using var client = new TernwireClient(apiKey, apiOrigin, TimeSpan.FromSeconds(30));
var message = await client.SubmitEmailAsync(new JsonObject
{
    ["from"] = "notifications@your-verified-domain.com",
    ["to"] = new JsonArray(JsonValue.Create("recipient@example.com")),
    ["subject"] = "Your receipt",
    ["text"] = "Thanks for your order."
}, "order-123-receipt");
```

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

Set `TERNWIRE_API_KEY`, `TERNWIRE_API_ORIGIN`, `TERNWIRE_FROM`, and `TERNWIRE_TO`
before deliberately running `dotnet run --project examples/Send`. It submits an email.

Use `https://ternwire.com` as the API origin. Read the full documentation at
[ternwire.com/docs](https://ternwire.com/docs), or contact support@ternwire.com.
Released under the MIT License.
