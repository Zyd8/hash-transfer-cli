class TransferUtils
{
    public static void CopyDirectory(string sourcePath, string destinationPath)
    {
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        foreach (string filePath in Directory.GetFiles(sourcePath))
        {
            string fileName = Path.GetFileName(filePath);
            string destPath = Path.Combine(destinationPath, fileName);
            File.Copy(filePath, destPath, true);
        }

        foreach (string subDirPath in Directory.GetDirectories(sourcePath))
        {
            string subDirName = Path.GetFileName(subDirPath);
            string destSubDir = Path.Combine(destinationPath, subDirName);
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

    public static void DoTransferOperation(TransferInfo TransferInfo)
    {
        if (!TransferInfo.IsSourceFile)
        {
            CopyDirectory(TransferInfo.Source, TransferInfo.Destination);
            return;
        }
        File.Copy(TransferInfo.Source, TransferInfo.Destination, true);
        return;
    }

    public static void RemoveDirectory(string directoryPath)
    {
        Directory.Delete(directoryPath, true);
    }
}