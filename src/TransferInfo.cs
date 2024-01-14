class TransferInfo
{
    private string _source = "";
    private string _destination = "";
    public bool IsSourceFile { get; set; }
    public TransferMode TransferMode { get; set; }
    public HashType HashType { get; set; }

    public TransferInfo(string source, string destination, TransferMode transferMode, HashType hashType)
    {
        Source = source;
        Destination = destination;
        TransferMode = transferMode;
        HashType = hashType;
    }

    public string Source
    {
        get { return _source; }
        set 
        {   
            try
            {
                if (File.Exists(value))
                {
                    IsSourceFile = true;
                    _source = value; 
                }
                else if (Directory.Exists(value))
                {
                    IsSourceFile = false;
                    _source = value;
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
        get { return _destination; }
        set 
        { 
            try
            {
                if (Directory.Exists(Path.GetDirectoryName(value)))
                {
                    _destination = value; 
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
}
