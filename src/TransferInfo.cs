class TransferInfo
{
    private string source = "";
    private bool isSourceFile;
    private string destination = "";
    private TransferMode transferMode;

    public TransferInfo(string source, string destination, TransferMode transferMode)
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
                Cleanup.InputErrorTermination();
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
                Cleanup.InputErrorTermination();
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
}
