using System.Security.Cryptography;
using System.Text;
using Core.Application.Abstractions;

namespace Core.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 16 bytes = 128 bits

    public string HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = ComputeHash(password, salt);
        return $"{salt}:{hash}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
        {
            throw new FormatException("The hashed password format is invalid.");
        }

        var salt = parts[0];
        var hash = parts[1];

        var computedHash = ComputeHash(password, salt);
        return hash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateSalt()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var saltBytes = new byte[SaltSize];
            rng.GetBytes(saltBytes);
            return BitConverter.ToString(saltBytes).Replace("-", "").ToLower();
        }
    }

    private string ComputeHash(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var passwordWithSaltBytes = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = sha256.ComputeHash(passwordWithSaltBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}