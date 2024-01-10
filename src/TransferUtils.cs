class TransferUtils
{
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

    public static void DoTransferOperation(TransferInfo TransferInfo)
    {
        TransferMode transferMode = TransferInfo.TransferMode;
        if (transferMode == TransferMode.copy)
        {
            if (!TransferInfo.IsSourceFile)
            {
                CopyDirectory(TransferInfo.Source, TransferInfo.Destination);
                return;
            }
            File.Copy(TransferInfo.Source, TransferInfo.Destination, true);
            return;

        }
        else if (transferMode == TransferMode.cut)
        {
            if (!TransferInfo.IsSourceFile)
            {
                Directory.Move(TransferInfo.Source, TransferInfo.Destination);
                return;
            }
            File.Move(TransferInfo.Source, TransferInfo.Destination);
            return;
        }
        else
        {
            throw new ArgumentException();
        }
    }
}