using System.Text.Json.Nodes;
using Ternwire;

static string Required(string key) => Environment.GetEnvironmentVariable(key)
    ?? throw new InvalidOperationException($"Set {key}");

using var client = new TernwireClient(Required("TERNWIRE_API_KEY"), Environment.GetEnvironmentVariable("TERNWIRE_API_ORIGIN"));
var accepted = await client.SubmitEmailAsync(new JsonObject
{
    ["from"] = Required("TERNWIRE_FROM"),
    ["to"] = new JsonArray(JsonValue.Create(Required("TERNWIRE_TO"))),
    ["subject"] = "Your receipt",
    ["text"] = "Thanks for your order."
}, "order-123-receipt");
Console.WriteLine(accepted?.ToJsonString());
