using System.Security.Cryptography;
using System.Text;

namespace Util.AuthorizationHelper.Util;

/// <summary>
/// 
/// </summary>
public static class HashHelper
{
    public static string GetMd5HashHexString(string input)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var hash = MD5.HashData(bytes);
        var hashString = BitConverter.ToString(hash).Replace("-", "");
        return hashString;
    }
}