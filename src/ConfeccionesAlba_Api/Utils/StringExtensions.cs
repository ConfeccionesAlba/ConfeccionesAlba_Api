using System.Security.Cryptography;
using System.Text;

namespace ConfeccionesAlba_Api.Utils;

/// <summary>
///     Provides extension methods for string manipulation and cryptographic operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Computes the SHA-256 hash of the input string.
    /// </summary>
    /// <param name="str">The input string to be hashed. Cannot be null.</param>
    /// <returns>A hexadecimal string representation of the SHA-256 hash.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input string is null.</exception>
    /// <remarks>
    ///     This method uses the SHA256 algorithm to generate a cryptographic hash.
    ///     The result is always a 64-character hexadecimal string.
    /// </remarks>
    /// <example>
    ///     <code>
    /// string input = "Hello, World!";
    /// string hash = input.GetSha256Hash();
    /// Console.WriteLine(hash); // Output: "dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f"
    /// </code>
    /// </example>
    public static string GetSha256Hash(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        return GetHashString(str);
    }

    /// <summary>
    ///     Computes the SHA-256 hash of the input string and returns it as a byte array.
    /// </summary>
    /// <param name="inputString">The input string to be hashed.</param>
    /// <returns>A byte array containing the SHA-256 hash.</returns>
    private static byte[] GetHash(string inputString)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
    }

    /// <summary>
    ///     Converts a byte array to its hexadecimal string representation.
    /// </summary>
    /// <param name="inputString">The input string used to generate the hash.</param>
    /// <returns>A hexadecimal string representation of the hash.</returns>
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