using System.Data.Common;
using System.Net.NetworkInformation;

class FileHashManager
{
    private List<FileInfo> _sourceInfo;
    private List<FileInfo> _destinationInfo;
    private List<(FileInfo sourceInfo, FileInfo destinationInfo)> _mismatchHashInfoPair;

    public List<FileInfo> SourceInfo
    {
        get { return _sourceInfo; }
        set { _sourceInfo = value; }
    }

    public List<FileInfo> DestinationInfo
    {
        get { return _destinationInfo; }
        set { _destinationInfo = value; }
    }

    public List<(FileInfo sourceInfo, FileInfo destinationInfo)> MismatchHashInfoPair
    {
        get { return _mismatchHashInfoPair; }
        set { _mismatchHashInfoPair = value; }
    }

    public FileHashManager()
    {
        _sourceInfo = new List<FileInfo>();
        _destinationInfo = new List<FileInfo>();
        _mismatchHashInfoPair = new List<(FileInfo, FileInfo)>();
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
                    Console.WriteLine($"[MISMATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfo.FilePath))}/{Path.GetFileName(sourceFileInfo.FilePath)}:\n{sourceFileInfo.HashValue}\n-->\n{destinationFileInfo.HashValue}");
                    MismatchHashInfoPair.Add((sourceFileInfo, destinationFileInfo));
                }
                else
                {
                    Console.WriteLine($"[MATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfo.FilePath))}/{Path.GetFileName(sourceFileInfo.FilePath)}:\n{sourceFileInfo.HashValue}\n-->\n{destinationFileInfo.HashValue}");
                }
            }
        }
        else 
        {
            List<(FileInfo sourceInfo, FileInfo destinationInfo)> updatedMismathHashInfoPairs = new List<(FileInfo, FileInfo)>();

            foreach (var pair in MismatchHashInfoPair)
            {
                if (pair.sourceInfo.HashValue != pair.destinationInfo.HashValue)
                {
                    Console.WriteLine($"[RECHECK MISMATCH] {Path.GetFileName(Path.GetDirectoryName(pair.sourceInfo.FilePath))}/{Path.GetFileName(pair.sourceInfo.FilePath)}:\n{pair.sourceInfo.HashValue}\n-->\n{pair.destinationInfo.HashValue}");
                    updatedMismathHashInfoPairs.Add((pair.sourceInfo, pair.destinationInfo));
                }
                else
                {
                    Console.WriteLine($"[RECHECK MATCH] {Path.GetFileName(Path.GetDirectoryName(pair.sourceInfo.FilePath))}/{Path.GetFileName(pair.sourceInfo.FilePath)}:\n{pair.sourceInfo.HashValue}\n-->\n{pair.destinationInfo.HashValue}");
                }
            }

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

    public void GetSourceInfoList(string sourcePath, HashType hashType)
    {
        List<FileInfo> fileInfoList = new List<FileInfo>();
        int fileInfoCtr = 0;

        if (!Directory.Exists(sourcePath))
        {

            FileInfo fileInfoObj = new FileInfo(sourcePath, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);

            foreach (FileInfo fileInfo in fileInfoList)
            {
                fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
            }

            SourceInfo = fileInfoList;
            return;
        }

        List<string> filePaths = TransferUtils.TraverseDirectories(sourcePath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
        }

        SourceInfo = fileInfoList;
    }

    public void GetDestinationInfoList(string destinationPath, HashType hashType)
    {
        List<FileInfo> fileInfoList = new List<FileInfo>();
        int fileInfoCtr = 0;

        if (!Directory.Exists(destinationPath))
        {
            FileInfo fileInfoObj = new FileInfo(destinationPath, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);

            foreach (FileInfo fileInfo in fileInfoList)
            {
                fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
            }

            DestinationInfo = fileInfoList;
            return;
        }

        List<string> filePaths = TransferUtils.TraverseDirectories(destinationPath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
        }

        DestinationInfo = fileInfoList;
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