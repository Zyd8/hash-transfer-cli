class Input
{

    public static TransferMode ParseTransferMode(string value)
    {
        if (string.Equals(value, "copy", StringComparison.OrdinalIgnoreCase))
        {
            return TransferMode.copy;
        }
        else if (string.Equals(value, "cut", StringComparison.OrdinalIgnoreCase))
        {
            return TransferMode.cut;
        }
        else
        {
            throw new ArgumentException("Invalid transfer operation chosen.");
        }
    }

    public static HashType ParseHashType(string value)
    {
        if (string.Equals(value, "md5", StringComparison.OrdinalIgnoreCase))
        {
            return HashType.MD5;
        }
        else if (string.Equals(value, "sha1", StringComparison.OrdinalIgnoreCase))
        {
            return HashType.SHA1;
        }
        else if (string.Equals(value, "sha256", StringComparison.OrdinalIgnoreCase))
        {
            return HashType.SHA256;
        }
        else if (string.Equals(value, "sha512", StringComparison.OrdinalIgnoreCase))
        {
            return HashType.SHA512;
        }
        else
        {
            throw new ArgumentException("Invalid hash algorithm.");
        }
    }

    public static string ParseLeadingSlash(string value)
    {
        if (value.StartsWith("/"))
        {
            return value.TrimStart('/');
        }
        return value;
    }
}