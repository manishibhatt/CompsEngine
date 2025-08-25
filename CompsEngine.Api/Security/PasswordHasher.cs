using System.Security.Cryptography;

public static class PasswordHasher
{
    public static (byte[] hash, byte[] salt) Hash(string password, int iterations = 100_000)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);            // 128-bit
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 64);
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] storedHash, byte[] storedSalt, int iterations = 100_000)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, storedSalt, iterations, HashAlgorithmName.SHA256, storedHash.Length);
        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}