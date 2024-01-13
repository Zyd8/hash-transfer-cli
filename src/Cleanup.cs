class Cleanup
{
    public static bool isSigintInvoked = false;
    public static string sourcePath = "";
    public static string destinationPath = "";
    public static int exceptionRecursiveCtr = 0;
    public static int exceptionRecursiveLimit = 3;

    public static void InputErrorTermination()
    {
        Environment.Exit(1);
    }

    public static void NoOverwriteFeedbackTermination()
    {
        Console.WriteLine("Application is called for termination...");
        Environment.Exit(0);
    }

    public static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        isSigintInvoked = true;
        Console.WriteLine("Application is called for termination...");
        if (destinationPath != string.Empty)
        {
            Console.WriteLine("Reverting changes...");
            RemoveDirectory(destinationPath);
        }
        Environment.Exit(0);
    }

    public static void OnCut()
    {
        RemoveDirectory(sourcePath);
    }

    public static void RemoveDirectory(string directoryPath)
    { 
        Directory.Delete(directoryPath, true);
    }
}