class HashTransferService
{
    private static void DoTransferOperation(SourceDestinationInfo SourceDestinationInfo)
    {
        TransferMode transferMode = SourceDestinationInfo.TransferMode;
        if (transferMode == TransferMode.copy)
        {
            if (!SourceDestinationInfo.IsSourceFile)
            {
                SourceDestination.CopyDirectory(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
                return;
            }
            File.Copy(SourceDestinationInfo.Source, SourceDestinationInfo.Destination, true);
            return;

        }
        else if (transferMode == TransferMode.cut)
        {
            if (!SourceDestinationInfo.IsSourceFile)
            {
                Directory.Move(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
                return;
            }
            File.Move(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
            return;

        }
        else
        {
            throw new ArgumentException(
                "Invalid state of SourceDestinationInfo.TransferMode or SourceDestinationInfo.IsSourceFile"
                );
        }

    }

    private static List<FileInfo> GetFileInfoList(string directoryPath, HashType hashType)
    {
        List<FileInfo> fileInfoList = new List<FileInfo>();
        int fileInfoCtr = 0;

        if (!Directory.Exists(directoryPath))
        {
            FileInfo fileInfoObj = new FileInfo(directoryPath, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);

            foreach (FileInfo fileInfo in fileInfoList)
            {
                fileInfo.HashValue = Hasher.GetFileHash(fileInfo.FilePath, hashType);
            }

            return fileInfoList;
        }

        List<string> filePaths = SourceDestination.TraverseDirectories(directoryPath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hasher.GetFileHash(fileInfo.FilePath, hashType);
        }

        return fileInfoList;
    }

    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("HashTransferCLI Usage: <transferMode> <hashType> <sourcePath> <destPath> ");
            return;
        }

        TransferMode transferMode = Parser.ParseTransferMode(args[0]);
        HashType hashType = Parser.ParseHashType(args[1]);
        string currentDirectory = Directory.GetCurrentDirectory();
        string fullSourcePath = Path.GetFullPath(Path.Combine(currentDirectory, Parser.ParseLeadingSlash(args[2])));
        string fullDestPath = Path.GetFullPath(Path.Combine(currentDirectory, Parser.ParseLeadingSlash(args[3]), Path.GetFileName(args[2])));

        SourceDestinationInfo SourceDestinationInfo = new(fullSourcePath, fullDestPath, transferMode);

        List<FileInfo> sourceFileInfoList = GetFileInfoList(SourceDestinationInfo.Source, hashType);

        DoTransferOperation(SourceDestinationInfo);

        List<FileInfo> destFileInfoList = GetFileInfoList(SourceDestinationInfo.Destination, hashType);

        int recopyCtr = 0;
        int recopyLimit = 3;
        CheckHashMismatch(sourceFileInfoList, destFileInfoList, SourceDestinationInfo, hashType, recopyCtr, recopyLimit);
    }

    private static void CheckHashMismatch(List<FileInfo> sourceFileInfoList, List<FileInfo> destFileInfoList, SourceDestinationInfo SourceDestinationInfo, HashType hashType, int recopyCtr, int recopyLimit)
    {
        var mismatchFileInfoList = SourceDestination.FindHashMismatch(sourceFileInfoList, destFileInfoList);

        if (SourceDestinationInfo.TransferMode == TransferMode.cut)
        {
            return;
        }

        if (recopyCtr >= recopyLimit)
        {
            throw new Exception("Files gets continously modified");
        }

        if (mismatchFileInfoList.Count != 0)
        {
            Console.WriteLine("There are file hashes that did not match");
            Console.WriteLine($"Attempting to recopy mismatched files...{recopyCtr += 1}");
            SourceDestination.FileRecover(mismatchFileInfoList, destFileInfoList);
            CheckHashMismatch(sourceFileInfoList, destFileInfoList, SourceDestinationInfo, hashType, recopyCtr, recopyLimit);
        }
        else
        {
            Console.WriteLine("All file hashes matched. File Transfer completed successfully!");
            return;
        }
    }
}

