﻿using CommandLine;

class HashTransferService
{
    private static HashTransferResult CheckHashMismatch(FileInfoManager fileInfoManager, TransferInfo transferInfo, int recopyCtr, int recopyLimit)
    {
        if (recopyCtr > recopyLimit)
        { 
            Console.WriteLine("Some file(s) seems to be continuously modified that hashes can't be matched");
            return HashTransferResult.mismatch;
        }

        fileInfoManager.FindHashMismatch();

        if (fileInfoManager.MismatchHashInfoPair.Count != 0)
        {
            Console.WriteLine("There are file hashes that did not match");

            Console.WriteLine($"Attempting to recopy mismatched files...{recopyCtr += 1}");

            fileInfoManager.MismatchHashResolver();

            fileInfoManager.UpdateHashInfoList(transferInfo.HashType);

            // Only recompares file info of mismatch source/destination file hashes
            return CheckHashMismatch(fileInfoManager, transferInfo, recopyCtr, recopyLimit); 
        }
        else
        {
            return HashTransferResult.match;
        }
    }

    // Handling of already existing destination path
    public static void HandleExistingDestination(TransferInfo transferInfo)
    {      
        if (Path.Exists(transferInfo.Destination))
        {
            if (!TransferUtils.isOverwrite(transferInfo.Destination))
            {
                Cleanup.NoOverwriteFeedbackTermination();
            }
        }
        Cleanup.SourcePath = transferInfo.Source;
        Cleanup.DestinationPath = transferInfo.Destination;
    }

    public static string GetSourceRelativeOrAbsolutePath(Options options)
    {
        // Absolute path
        if (Path.IsPathRooted(options.InputSourcePath))
        {
           return Input.RemoveEndSlash(options.InputSourcePath);
        }
        // Relative path
        return Path.GetFullPath(Input.RemoveEndSlash(options.InputSourcePath));
    }

    public static string GetDestinationRelativeOrAbsolutePath(Options options)
    {
        // Absolute path
        if (Path.IsPathRooted(options.InputDestinationPath))
        {
            return Path.Combine(options.InputDestinationPath, Path.GetFileName(Input.RemoveEndSlash(options.InputSourcePath)));
        }
        // Relative path
        return Path.Combine(Path.GetFullPath(options.InputDestinationPath), Path.GetFileName(Input.RemoveEndSlash(options.InputSourcePath)));
    }

    static void Main(string[] args)
    {
        Console.CancelKeyPress += (sender, args) => Cleanup.OnCancelKeyPress(sender, args);

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
        {
                string fullSourcePath = GetSourceRelativeOrAbsolutePath(options);
                string fullDestinationPath = GetDestinationRelativeOrAbsolutePath(options);   
                TransferMode transferMode = Input.ParseTransferMode(options.InputTransferMode);
                HashType hashType = Input.ParseHashType(options.InputHashType);

                TransferInfo transferInfo = new(fullSourcePath, fullDestinationPath, transferMode, hashType);
                FileInfoManager fileInfoManager = new();

                HandleExistingDestination(transferInfo);

                try
                {
                    Task task1 = Task.Run(() =>
                    {
                        Console.WriteLine("Fetching source file hashes...");
                        fileInfoManager.GetSourceInfoList(transferInfo);
                    });

                    Task task2 = Task.Run(() =>
                    {
                        Console.WriteLine("Transferring files...");
                        TransferUtils.DoTransferOperation(transferInfo);
                    });

                    Task.WaitAll(task1, task2);

                    Console.WriteLine("Fetching destination file hashes...");
                    fileInfoManager.GetDestinationInfoList(transferInfo);

                    Console.WriteLine("Comparing file hashes...");
                    int recopyCtr = 0; int recopyLimit = 3;
                    HashTransferResult result = CheckHashMismatch(fileInfoManager, transferInfo, recopyCtr, recopyLimit);

                    if (result == HashTransferResult.match)
                    {
                        if (transferInfo.TransferMode == TransferMode.cut)
                        {
                            Cleanup.OnCut();
                        }
                        Console.WriteLine($"All {fileInfoManager.SourceInfo.Count} files hashes matched. File transfer completed successfully!");  
                    }
                    else if (result == HashTransferResult.mismatch)
                    {
                        Console.WriteLine("There are file hashes that did not match.");
                    }
                }
                catch (Exception e)
                {
                    if (Cleanup.ExceptionRecursiveCtr >= Cleanup.ExceptionRecursiveLimit)
                    {
                        throw new Exception($"Recursive exception limit reached: {e.Message}");
                    }
                    if (Cleanup.IsSigintInvoked)
                    {
                        Console.WriteLine($"Exception raised due to SIGINT, continuing cleanup");
                        Cleanup.ExceptionRecursiveCtr += 1;
                        Cleanup.RemoveDirectory(Cleanup.DestinationPath);
                    }
                    Console.WriteLine($"An unexpected error occured:{e.Message}");
                }
        });
    }
}

