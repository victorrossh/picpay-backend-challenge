using System.Security.Cryptography;
using System.Text;
using Teste.Application.Services.Implementations;

namespace Teste.Application.Services;

public class CryptographyService : ICryptographyImp
{
    public string EncryptPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = GenerateHash(password, salt, 100000);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var hashAttempt = GenerateHash(password, salt, 100000);
        return storedHash.SequenceEqual(hashAttempt);
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(salt);
        return salt;
    }

    private static byte[] GenerateHash(string password, byte[] salt, int iterations)
    {
        using var sha256 = SHA256.Create();
        var input = new byte[password.Length + salt.Length];
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(password), 0, input, 0, password.Length);
        Buffer.BlockCopy(salt, 0, input, password.Length, salt.Length);

        var hash = input;
        for (var i = 0; i < iterations; i++)
            if (sha256 != null)
                hash = sha256.ComputeHash(hash);

        return hash;
    }
}