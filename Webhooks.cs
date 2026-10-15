using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Ternwire;

public static class Webhooks
{
    public static bool Verify(string header, ReadOnlySpan<byte> body, string secret, long toleranceSeconds = 300)
    {
        if (string.IsNullOrEmpty(secret) || toleranceSeconds < 0) return false;
        var match = Regex.Match(header, @"\A\s*t=([1-9][0-9]{0,11})\s*,\s*v1=([a-f0-9]{64})\s*\z");
        if (!match.Success) return false;
        var timestamp = long.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        if (Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp) > toleranceSeconds) return false;
        var prefix = Encoding.ASCII.GetBytes(match.Groups[1].Value + ".");
        using var hmac = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA256, Encoding.UTF8.GetBytes(secret));
        hmac.AppendData(prefix);
        hmac.AppendData(body);
        return CryptographicOperations.FixedTimeEquals(hmac.GetHashAndReset(), Convert.FromHexString(match.Groups[2].Value));
    }
}
