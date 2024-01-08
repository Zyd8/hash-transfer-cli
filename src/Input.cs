class Input
{

    public static TransferMode ParseTransferMode(string value)
    {
        try
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
                throw new ArgumentException("Invalid transfer operation chosen. Please enter either 'copy' or 'cut'.");
            }
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1); 
            return TransferMode.copy; // to satisfy the compiler 
        }
    }


    public static HashType ParseHashType(string value)
    {
        try 
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
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1); 
            return HashType.MD5; // to satisfy the compiler 
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