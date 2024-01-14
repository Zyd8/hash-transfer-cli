class TransferInfo
{
    private string _source = "";
    private bool _isSourceFile;
    private string _destination = "";
    private TransferMode _transferMode;
    private HashType _hashType;

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
    
    public bool IsSourceFile
    {
        get { return _isSourceFile; }
        set
        {
            _isSourceFile = value;
        }
    }

    public TransferMode TransferMode
    {
        get { return _transferMode; }
        set
        {
            _transferMode = value;
        }
    }

    public HashType HashType
    {
        get { return _hashType; }
        set
        {
            _hashType = value;
        }
    }
}
