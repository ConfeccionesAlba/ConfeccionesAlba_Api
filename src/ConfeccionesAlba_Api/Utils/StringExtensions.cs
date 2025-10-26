using System.Security.Cryptography;
using System.Text;

namespace ConfeccionesAlba_Api.Utils;

public static class StringExtensions
{
    public static string GetSha256Hash(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);
        
        return GetHashString(str);
    }

    private static byte[] GetHash(string inputString)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
    }

    private static string GetHashString(string inputString)
    {
        var sb = new StringBuilder();
        foreach (var b in GetHash(inputString))
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}