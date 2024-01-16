class Cleanup
{
    public static bool IsSigintInvoked { get; set; } = false;
    public static string SourcePath { get; set; } = "";
    public static string DestinationPath { get; set; } = "";
    public static int ExceptionRecursiveRetryCtr { get; set; } = 0;
    public static int ExceptionRecursiveRetryLimit { get; set; } = 3;

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
        Console.WriteLine("Reverting changes...");
        RemoveDestination();
        Environment.Exit(0);
    }

    public static void CheckExceptionRecursiveRetryReached(Exception e)
    {
        if (ExceptionRecursiveRetryCtr >= ExceptionRecursiveRetryLimit)
        {
            throw new Exception($"Recursive exception limit reached: {e.Message}");
        }
    }

    public static void DirectoryNotFoundException(DirectoryNotFoundException e)
    {
        CheckExceptionRecursiveRetryReached(e);
        if (IsSigintInvoked)
        {
            Console.WriteLine($"Expected Directory not found Exception raised due to SIGINT, continuing cleanup");
            ExceptionRecursiveRetryCtr += 1;
            RemoveDestination();
            Environment.Exit(0);
        }
        Console.WriteLine($"Directory not found: {e.Message}");
    }

    public static void FileNotFoundException(FileNotFoundException e)
    {
        CheckExceptionRecursiveRetryReached(e);
        if (IsSigintInvoked)
        {
            Console.WriteLine($"Expected File not found Exception raised due to SIGINT, continuing cleanup");
            ExceptionRecursiveRetryCtr += 1;
            RemoveDestination();
            Environment.Exit(0);
        }
        Console.WriteLine($"File not found: {e.Message}");
    }

    public static void UnauthorizedAccessException(UnauthorizedAccessException e)
    {
        CheckExceptionRecursiveRetryReached(e);
        if (IsSigintInvoked)
        {
            Console.WriteLine($"Unauthorized Access Exception raised due to SIGINT, continuing cleanup");
            ExceptionRecursiveRetryCtr += 1;
            RemoveDestination();
            Environment.Exit(0);
        }
        Console.WriteLine(e.Message);
        Console.WriteLine("You have no permission to modify any of the provided paths. Try running again with elevated privileges.");
        Environment.Exit(1);
    }

    public static void IOException(IOException e)
    {
        CheckExceptionRecursiveRetryReached(e);
        if (IsSigintInvoked)
        {
            Console.WriteLine($"Input/Output Exception raised due to SIGINT, continuing cleanup");
            ExceptionRecursiveRetryCtr += 1;
            RemoveDestination();
            Environment.Exit(0);
        }
        Console.WriteLine($"Input/Output error: {e.Message}");
    }

    public static void RemoveSource()
    {
        if (Path.Exists(SourcePath))
        {
            TransferUtils.RemoveDirectory(SourcePath);
        }
    }

    public static void RemoveDestination()
    {
        if (Path.Exists(DestinationPath))
        {
            TransferUtils.RemoveDirectory(DestinationPath);
        }
    }
}