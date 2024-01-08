using System.Security.Cryptography;

class Hasher
{
    public static string GetFileHash(string path, HashType hashType)
    {
        using (var stream = File.OpenRead(path))
        {
            using (var hashAlgorithm = GetHashAlgorithm(hashType))
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    private static HashAlgorithm GetHashAlgorithm(HashType hashType)
    {
        switch (hashType)
        {
            case HashType.MD5:
                return MD5.Create();
            case HashType.SHA1:
                return SHA1.Create();
            case HashType.SHA256:
                return SHA256.Create();
            case HashType.SHA512:
                return SHA512.Create();
            default:
                throw new Exception();
        }
    }
}

