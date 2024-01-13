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

    public static void DoTransferOperation(TransferInfo transferInfo)
    {
        try 
        {
            if (!transferInfo.IsSourceFile)
            {
                CopyDirectory(transferInfo.Source, transferInfo.Destination);
                return;
            }
            File.Copy(transferInfo.Source, transferInfo.Destination, true);
            return;
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("You have no permission to modify any of the provided paths. Try running with elevated privileges.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred during transfer operation: {e.Message}");
        }
    }

    public static bool isOverwrite(string path)
    {
        Console.Write("Path provided currently exists in the destination. Overwrite? (y/n): ");
        string? ans = Console.ReadLine();
        if (string.Equals(ans, "y", StringComparison.OrdinalIgnoreCase) || string.Equals(ans, "yes", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else if (string.Equals(ans, "n", StringComparison.OrdinalIgnoreCase) || string.Equals(ans, "no", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            return isOverwrite(path);
        }
    }

    public static void RemoveDirectory(string directoryPath)
    {
        try
        {
            Directory.Delete(directoryPath, true);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting directory: {e.Message}");
        }
    }
}