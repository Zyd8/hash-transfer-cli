class SourceDestination
{
    public static List<FileInfo> FindHashMismatch(List<FileInfo> sourceFileInfoList, List<FileInfo> destFileInfoList)
    {
        List<FileInfo> mismatchFileInfoList = new List<FileInfo>();

        if (sourceFileInfoList.Count != destFileInfoList.Count)
        {
            throw new Exception("External processes affected the program");
        }

        for (int i = 0; i < sourceFileInfoList.Count; i++)
        {
            for (int j = 0; j < destFileInfoList.Count; j++)
            {
                if (sourceFileInfoList[i].Key == destFileInfoList[j].Key)
                {
                    if (sourceFileInfoList[i].HashValue == destFileInfoList[j].HashValue)
                    {
                        Console.WriteLine($"[MATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfoList[i].FilePath))}/{Path.GetFileName(sourceFileInfoList[i].FilePath)} : {sourceFileInfoList[i].HashValue} --> {destFileInfoList[j].HashValue}");
                    }
                    else
                    {
                        Console.WriteLine($"[MISMATCH] {Path.GetFileName(Path.GetDirectoryName(sourceFileInfoList[i].FilePath))}/{Path.GetFileName(sourceFileInfoList[i].FilePath)} : {sourceFileInfoList[i].HashValue} --> {destFileInfoList[j].HashValue}");
                        mismatchFileInfoList.Add(sourceFileInfoList[i]);
                    }
                }
            }
        }

        return mismatchFileInfoList;
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

    public static void FileRecover(List<FileInfo> mismatchFileInfoList, List<FileInfo> destFileInfoList)
    {
        for (int i = 0; i < mismatchFileInfoList.Count; i++)
        {
            for (int j = 0; j < destFileInfoList.Count; j++)
            {
                if (mismatchFileInfoList[i].Key == destFileInfoList[j].Key)
                {
                    File.Copy(mismatchFileInfoList[i].FilePath, destFileInfoList[j].FilePath, true);
                }
            }
        }
    }

}