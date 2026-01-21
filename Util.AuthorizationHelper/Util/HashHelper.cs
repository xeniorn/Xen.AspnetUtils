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
}