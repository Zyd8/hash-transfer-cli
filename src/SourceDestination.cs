class SourceDestination
{
    public static List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)> FindHashMismatch(
        List<FileInfo> sourceFileInfoList, List<FileInfo> destFileInfoList)
    {
        List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)> mismatchFilePairs = 
            new List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)>();

        for (int i = 0; i < sourceFileInfoList.Count; i++)
        {
            for (int j = 0; j < destFileInfoList.Count; j++)
            {
                if (sourceFileInfoList[i].Key == destFileInfoList[j].Key)
                {
                    if (sourceFileInfoList[i].HashValue != destFileInfoList[j].HashValue)
                    {
                        Console.WriteLine($"[MISMATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfoList[i].FilePath))}/{Path.GetFileName(sourceFileInfoList[i].FilePath)} : {sourceFileInfoList[i].HashValue} --> {destFileInfoList[j].HashValue}");
                        mismatchFilePairs.Add((sourceFileInfoList[i], destFileInfoList[j]));
                    }
                    else
                    {
                        Console.WriteLine($"[MATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfoList[i].FilePath))}/{Path.GetFileName(sourceFileInfoList[i].FilePath)} : {sourceFileInfoList[i].HashValue} --> {destFileInfoList[j].HashValue}");
                    }
                }
            }
        }

        return mismatchFilePairs;
    }

    public static void CopyDirectory(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destPath = Path.Combine(destDir, fileName);
            File.Copy(filePath, destPath, true);
        }

        foreach (string subDirPath in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDirPath);
            string destSubDir = Path.Combine(destDir, subDirName);
            CopyDirectory(subDirPath, destSubDir);
        }
    }

    public static List<string> TraverseDirectories(string directoryPath)
    {
        List<string> filePaths = new List<string>();

        foreach (string filePath in Directory.GetFiles(directoryPath))
        {
            filePaths.Add(filePath);
        }

        foreach (string subDirPath in Directory.GetDirectories(directoryPath))
        {
            filePaths.AddRange(TraverseDirectories(subDirPath));
        }

        return filePaths;
    }

    public static void FileRecover(List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)> mismatchFilePairs)
    {
        foreach (var mismatchPair in mismatchFilePairs)
        {
            File.Copy(mismatchPair.SourceFileInfo.FilePath, mismatchPair.DestFileInfo.FilePath, true);
        }
    }
}