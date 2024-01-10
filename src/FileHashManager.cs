using System.Data.Common;
using System.Net.NetworkInformation;

class FileHashManager
{
    public List<FileInfo> sourceInfo;
    public List<FileInfo> destinationInfo;
    public List<(FileInfo sourceInfo, FileInfo destinationInfo)> mismatchHashInfoPair;

    public FileHashManager()
    {
        sourceInfo = new List<FileInfo>();
        destinationInfo = new List<FileInfo>();
        mismatchHashInfoPair = new List<(FileInfo, FileInfo)>();
    } 

    public void FindHashMismatch()
    {
        if (mismatchHashInfoPair.Count == 0)
        {
            for (int i = 0; i < sourceInfo.Count; i++)
            {
                for (int j = 0; j < destinationInfo.Count; j++)
                {
                    if (sourceInfo[i].Key == destinationInfo[j].Key)
                    {
                        if (sourceInfo[i].HashValue != destinationInfo[j].HashValue)
                        {
                            Console.WriteLine($"[MISMATCH] {Path.GetFileName(Path.GetDirectoryName(sourceInfo[i].FilePath))}/{Path.GetFileName(sourceInfo[i].FilePath)} : {sourceInfo[i].HashValue} --> {destinationInfo[j].HashValue}");
                            mismatchHashInfoPair.Add((sourceInfo[i], destinationInfo[j]));
                        }
                        else
                        {
                            Console.WriteLine($"[MATCH] {Path.GetFileName(Path.GetDirectoryName(sourceInfo[i].FilePath))}/{Path.GetFileName(sourceInfo[i].FilePath)} : {sourceInfo[i].HashValue} --> {destinationInfo[j].HashValue}");
                        }
                    }

                }
            }
        }
        else 
        {
            List<(FileInfo sourceInfo, FileInfo destinationInfo)> updatedMismathHashInfoPairs = new List<(FileInfo, FileInfo)>();

            foreach (var pair in mismatchHashInfoPair)
            {
                if (pair.sourceInfo.HashValue != pair.destinationInfo.HashValue)
                {
                    Console.WriteLine($"[RECHECK MISMATCH] {Path.GetFileName(Path.GetDirectoryName(pair.sourceInfo.FilePath))}/{Path.GetFileName(pair.sourceInfo.FilePath)} : {pair.sourceInfo.HashValue} --> {pair.destinationInfo.HashValue}");
                    updatedMismathHashInfoPairs.Add((pair.sourceInfo, pair.destinationInfo));
                }
                else
                {
                    Console.WriteLine($"[RECHECK MATCH] {Path.GetFileName(Path.GetDirectoryName(pair.sourceInfo.FilePath))}/{Path.GetFileName(pair.sourceInfo.FilePath)} : {pair.sourceInfo.HashValue} --> {pair.destinationInfo.HashValue}");
                }
            }

            mismatchHashInfoPair = updatedMismathHashInfoPairs;
        }
    }

    public void MismatchHashResolver()
    {
        foreach (var pair in mismatchHashInfoPair)
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

            sourceInfo = fileInfoList;
        }

        List<string> filePaths = TransferManager.TraverseDirectories(sourcePath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
        }

        sourceInfo = fileInfoList;
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

            destinationInfo = fileInfoList;
        }

        List<string> filePaths = TransferManager.TraverseDirectories(destinationPath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hash.GetFileHash(fileInfo.FilePath, hashType);
        }

        destinationInfo = fileInfoList;
    }

    public void UpdateHashInfoList(HashType hashType)
    {
        foreach (var pair in mismatchHashInfoPair)
        {
            int mismatchSourceKey = pair.sourceInfo.Key;
            int mismatchDestinationKey = pair.destinationInfo.Key;

            FileInfo sourceHashInfoToUpdate = sourceInfo.Find(info => info.Key == mismatchSourceKey)!;
            FileInfo destinationHashInfoToUpdate = destinationInfo.Find(info => info.Key == mismatchDestinationKey)!;

            if (sourceHashInfoToUpdate != null)
            {
                sourceHashInfoToUpdate.HashValue = Hash.GetFileHash(sourceHashInfoToUpdate.FilePath, hashType);
            }
            if (destinationHashInfoToUpdate != null)
            {
                destinationHashInfoToUpdate.HashValue = Hash.GetFileHash(destinationHashInfoToUpdate.FilePath, hashType);
            }
        }
    }
}