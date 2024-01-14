class Cleanup
{
    public static bool IsSigintInvoked { get; set; } = false;
    public static string SourcePath { get; set; } = "";
    public static string DestinationPath { get; set; } = "";
    public static int ExceptionRecursiveCtr { get; set; } = 0;
    public static int ExceptionRecursiveLimit { get; set; } = 3;

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
        IsSigintInvoked = true;
        Console.WriteLine("Application is called for termination...");
        if (DestinationPath != string.Empty)
        {
            Console.WriteLine("Reverting changes...");
            RemoveDirectory(DestinationPath);
        }
        Environment.Exit(0);
    }

    public static void OnCut()
    {
        RemoveDirectory(SourcePath);
    }

    public static void RemoveDirectory(string directoryPath)
    { 
        Directory.Delete(directoryPath, true);
    }
}