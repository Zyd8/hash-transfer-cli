class Cleanup
{
    public static bool isSigintInvoked;

    public static string pathToRemove = "";

    public static int exceptionRecursiveCtr = 0;
    public static int exceptionRecursiveLimit = 3;

    public static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("Application is called for termination...");
        if (pathToRemove != string.Empty)
        {
            Console.WriteLine("Reverting changes...");
            RemoveDirectory(pathToRemove);
        }
        Environment.Exit(0);
    }

    public static void NoOverwriteFeedbackTermination()
    {
        Console.WriteLine("Application is called for termination...");
        Environment.Exit(0);
    }

    public static void RemoveDirectory(string directoryPath)
    {
        try
        {   
            Directory.Delete(directoryPath, true);
        }
        catch (Exception e)
        {
            if (exceptionRecursiveCtr >= exceptionRecursiveLimit)
            {
                throw new Exception($"An unexpected error occured: {e.Message}");
            }
            if (isSigintInvoked)
            {
                Console.WriteLine($"Exception raised due to SIGINT, continuing cleanup");
                exceptionRecursiveCtr += 1;
                RemoveDirectory(pathToRemove);
            }
            Console.WriteLine($"Error deleting directory: {e.Message}");
        }
    }
}