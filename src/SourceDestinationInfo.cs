class SourceDestinationInfo
{
    private string source = "";
    private bool isSourceFile;
    private string destination = "";
    private TransferMode transferMode;

    public SourceDestinationInfo(string source, string destination, TransferMode transferMode)
    {
        Source = source;
        Destination = destination;
        TransferMode = transferMode;
    }

    public string Source
    {
        get { return source; }
        set 
        {   
            try
            {
                if (File.Exists(value))
                {
                    isSourceFile = true;
                    source = value; 
                }
                else if (Directory.Exists(value))
                {
                    isSourceFile = false;
                    source = value;
                }
                else 
                {
                    throw new Exception($"Source instance [{value}] is neither an existing file nor a directory.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1); 
            }

        }
    }

    public string Destination
    {
        get { return destination; }
        set 
        { 
            try
            {
                if (Directory.Exists(Path.GetDirectoryName(value)))
                {
                    destination = value; 
                }
                else 
                {
                    throw new DirectoryNotFoundException($"Destination instance [{value}] directory does not exist.");
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1); 
            }
        }
    }
    
    public bool IsSourceFile
    {
        get { return isSourceFile; }
        set
        {
            isSourceFile = value;
        }
    }

    public TransferMode TransferMode
    {
        get { return transferMode; }
        set
        {
            transferMode = value;
        }
    }

    private static void Display(List<FileInfo> fileInfoList)
    {
        foreach (FileInfo fileInfo in fileInfoList)
        {
            Console.WriteLine($"File Info:");
            Console.WriteLine($"  Key: {fileInfo.Key}");
            Console.WriteLine($"  File Path: {fileInfo.FilePath}");
            Console.WriteLine($"  Hash Value: {fileInfo.HashValue}");
        }
    }
}
