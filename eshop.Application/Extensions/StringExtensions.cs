using System.Security.Cryptography;
using System.Text;

namespace Auth.API.Extensions;

public static class StringExtensions
{
    public static string HashedToken(this string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}