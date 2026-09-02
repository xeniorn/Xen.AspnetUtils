using System.Security.Cryptography;
using System.Text;

namespace Util.AuthorizationHelper.Util;

/// <summary>
/// Utilities for dealing with hashes
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Creates an md5 hash from input string.
    /// String will be treated as ascii encoded. Output will be lowercase by default.
    /// blaZaraBla123 => 4111d3bd7d3bd2842ad0e84b77290e0f 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="lowercase"></param>
    /// <returns></returns>
    public static string GetMd5HashHexString(string input, bool lowercase = true)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var hash = MD5.HashData(bytes);
        var hashString = BitConverter.ToString(hash).Replace("-", "");
        return lowercase
            ? hashString.ToLower()
            : hashString.ToUpper();
    }

    /// <summary>
    /// Creates a sha256 hash from input string.
    /// String will be treated as utf8 encoded. Output will be lowercase by default.
    /// blaZaraBla123 => ab91fe8a7400494d01674d2933d3587227ddbb92c92ca30d0c31f4d97f34be3f
    /// </summary>
    /// <remarks>
    /// Deliberately utf8, unlike <see cref="GetMd5HashHexString"/>, which is ascii and therefore maps every
    /// non-ascii character to the same byte - two different inputs could hash alike. Both behave identically
    /// for ascii input, which is what api keys are, so this is not a compatibility concern in practice.
    /// </remarks>
    /// <param name="input"></param>
    /// <param name="lowercase"></param>
    /// <returns></returns>
    public static string GetSha256HashHexString(string input, bool lowercase = true)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        var hashString = Convert.ToHexString(hash);
        return lowercase
            ? hashString.ToLowerInvariant()
            : hashString.ToUpperInvariant();
    }
}