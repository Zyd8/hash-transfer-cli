class FileInfoManager
{
    public List<FileInfo> SourceInfo { get; set; }
    public List<FileInfo> DestinationInfo { get; set; }
    public List<(FileInfo sourceInfo, FileInfo destinationInfo)> MismatchHashInfoPair { get; set; }

    public FileInfoManager()
    {
        SourceInfo = new List<FileInfo>();
        DestinationInfo = new List<FileInfo>();
        MismatchHashInfoPair = new List<(FileInfo, FileInfo)>();
    }

    public void FindHashMismatch()
    {
        if (MismatchHashInfoPair.Count == 0)
        {
            var sourceDictionary = SourceInfo.ToDictionary(info => info.Key);
            var destinationDictionary = DestinationInfo.ToDictionary(info => info.Key);

            foreach (var key in sourceDictionary.Keys.Intersect(destinationDictionary.Keys))
            {
                var sourceFileInfo = sourceDictionary[key];
                var destinationFileInfo = destinationDictionary[key];

                if (sourceFileInfo.HashValue != destinationFileInfo.HashValue)
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine($"[MISMATCH]\n{sourceFileInfo.FilePath}:\n{sourceFileInfo.HashValue}\n-->\n{destinationFileInfo.FilePath}\n{destinationFileInfo.HashValue}");
                    MismatchHashInfoPair.Add((sourceFileInfo, destinationFileInfo));
                }
                else
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine($"[MATCH]\n{sourceFileInfo.FilePath}:\n{sourceFileInfo.HashValue}\n-->\n{destinationFileInfo.FilePath}\n{destinationFileInfo.HashValue}");
                }
            }
            Console.WriteLine("------------------------------------------------------------------------------------");
        }
        else 
        {
            List<(FileInfo sourceInfo, FileInfo destinationInfo)> updatedMismathHashInfoPairs = new List<(FileInfo, FileInfo)>();

            foreach (var pair in MismatchHashInfoPair)
            {
                if (pair.sourceInfo.HashValue != pair.destinationInfo.HashValue)
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine($"[RECHECK MISMATCH] {pair.sourceInfo.FilePath}:\n{pair.sourceInfo.HashValue}\n-->\n{pair.destinationInfo.FilePath}\n{pair.destinationInfo.HashValue}");
                    updatedMismathHashInfoPairs.Add((pair.sourceInfo, pair.destinationInfo));
                }
                else
                {
                    Console.WriteLine("------------------------------------------------------------------------------------");
                    Console.WriteLine($"[RECHECK MATCH] {pair.sourceInfo.FilePath}:\n{pair.sourceInfo.HashValue}\n-->\n{pair.destinationInfo.FilePath}\n{pair.destinationInfo.HashValue}");
                }
            }
            Console.WriteLine("------------------------------------------------------------------------------------");
            MismatchHashInfoPair = updatedMismathHashInfoPairs;
        }
    }

    public void MismatchHashResolver()
    {
        foreach (var pair in MismatchHashInfoPair)
        {
            File.Copy(pair.sourceInfo.FilePath, pair.destinationInfo.FilePath, true);
        }
    }

    private List<FileInfo> ProcessInfo(string path, HashType hashType)
    {
        List<FileInfo> fileInfoList = new List<FileInfo>();
        int fileInfoCtr = 0;

        if (!Directory.Exists(path))
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);

            foreach (FileInfo fileInfo in fileInfoList)
            {
                fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
            }

            return fileInfoList;
        }

        List<string> filePaths = TransferUtils.TraverseDirectories(path);

        var sortedFilePaths = filePaths.OrderBy(filePath => filePath).ToList();

        foreach (var filePath in sortedFilePaths)
        {
            FileInfo fileInfoObj = new FileInfo(filePath, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
        }

        return fileInfoList;
    }

    public void GetSourceInfoList(TransferInfo transferInfo)
    {
        SourceInfo = ProcessInfo(transferInfo.Source, transferInfo.HashType);
    }

    public void GetDestinationInfoList(TransferInfo transferInfo)
    {
        DestinationInfo = ProcessInfo(transferInfo.Destination, transferInfo.HashType);
    }

    public void UpdateHashInfoList(HashType hashType)
    {
        foreach (var pair in MismatchHashInfoPair)
        {
            int mismatchSourceKey = pair.sourceInfo.Key;
            int mismatchDestinationKey = pair.destinationInfo.Key;

            FileInfo sourceHashInfoInstance = SourceInfo.Find(info => info.Key == mismatchSourceKey)!;
            FileInfo destinationHashInfoInstance = DestinationInfo.Find(info => info.Key == mismatchDestinationKey)!;

            if (sourceHashInfoInstance != null)
            {
                sourceHashInfoInstance.HashValue = Hash.GetFileHash(sourceHashInfoInstance.FilePath, hashType);
            }
            if (destinationHashInfoInstance != null)
            {
                destinationHashInfoInstance.HashValue = Hash.GetFileHash(destinationHashInfoInstance.FilePath, hashType);
            }
        }
    }
}