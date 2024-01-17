using System.Security.Cryptography;
using System.IO.Hashing;
class Hash
{
    public static string GetFileHash(string path, HashType hashType)
    {
        byte[] fileBytes = File.ReadAllBytes(path);

        if (hashType == HashType.CRC32)
        { 
            Crc32 crc32 = new Crc32();
            Span<byte> hashBytes = new byte[crc32.HashLengthInBytes];
            Crc32.Hash(fileBytes, hashBytes);

            return BitConverter.ToString(hashBytes.ToArray()).Replace("-", "").ToLower();
        }
        else if (hashType == HashType.CRC64)
        {
            Crc64 crc64 = new Crc64();
            Span<byte> hashBytes = new byte[crc64.HashLengthInBytes];
            Crc64.Hash(fileBytes, hashBytes);

            return BitConverter.ToString(hashBytes.ToArray()).Replace("-", "").ToLower();
        }

        using (var hashAlgorithm = GetCryptographicHashAlgorithm(hashType))
        {
            byte[] hashBytes = hashAlgorithm.ComputeHash(fileBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    private static HashAlgorithm GetCryptographicHashAlgorithm(HashType hashType)
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

