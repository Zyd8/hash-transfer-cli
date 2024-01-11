
using CommandLine;

class HashTransferService
{
    private static HashTransferResult CheckHashMismatch(FileHashManager fileHashManager, TransferInfo transferInfo, HashType hashType, int recopyCtr, int recopyLimit)
    {
        if (recopyCtr > recopyLimit)
        { 
            Console.WriteLine("Some file(s) seems to be continuously modified that hashes can't be matched");
            return HashTransferResult.mismatch;
        }

        fileHashManager.FindHashMismatch();

        if (fileHashManager.mismatchHashInfoPair.Count != 0)
        {
            Console.WriteLine("There are file hashes that did not match");
            Console.WriteLine($"Attempting to recopy mismatched files...{recopyCtr += 1}");
            fileHashManager.MismatchHashResolver();
            fileHashManager.UpdateHashInfoList(hashType);
            // Only recompares file info of mismatch source and destination file hashes
            return CheckHashMismatch(fileHashManager, transferInfo, hashType, recopyCtr, recopyLimit); // Recursive
        }
        else
        {
            return HashTransferResult.match;
        }
    }

    public class Options
    {
        [Option('m', "mode", Required = false, Default = "copy", HelpText = "Transfer mode")]
        public required string InputTransferMode {get; set;}

        [Option('h', "hash", Required = false, Default = "MD5", HelpText = "Hash Algorithm")]
        public required string InputHashType {get; set;}

        [Value(0, Required = true, HelpText = "Source path")]
        public required string InputSourcePath {get; set;}

        [Value(1, Required = true, HelpText = "Destination path")]
        public required string InputDestinationPath {get; set;}
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
        {
                string fullSourcePath;
                if (Path.IsPathRooted(options.InputSourcePath))
                {
                    fullSourcePath = options.InputSourcePath;
                }
                else
                {
                    fullSourcePath = Path.GetFullPath(options.InputSourcePath);
                }

                string fullDestinationPath;
                if (Path.IsPathRooted(options.InputDestinationPath))
                {
                    fullDestinationPath = Path.Combine(options.InputDestinationPath, Path.GetFileName(options.InputSourcePath));
                }
                else
                {
                    fullDestinationPath = Path.Combine(Path.GetFullPath(options.InputDestinationPath), Path.GetFileName(options.InputSourcePath));
                }
               
                TransferMode transferMode = Input.ParseTransferMode(options.InputTransferMode);

                HashType hashType = Input.ParseHashType(options.InputHashType);

                TransferInfo transferInfo = new(fullSourcePath, fullDestinationPath, transferMode);

                FileHashManager fileHashManager = new();

                Task task1 = Task.Run(() =>
                {
                    Console.WriteLine("Fetching source file hashes...");
                    fileHashManager.GetSourceInfoList(transferInfo.Source, hashType);
                });

                Task task2 = Task.Run(() =>
                {
                    Console.WriteLine("Transferring files...");
                    TransferUtils.DoTransferOperation(transferInfo);
                });

                Task.WaitAll(task1, task2);

                Console.WriteLine("Fetching destination file hashes...");
                fileHashManager.GetDestinationInfoList(transferInfo.Destination, hashType);

                Console.WriteLine("Comparing file hashes...");
                int recopyCtr = 0; int recopyLimit = 30; // test value
                HashTransferResult result = CheckHashMismatch(fileHashManager, transferInfo, hashType, recopyCtr, recopyLimit);

                if (result == HashTransferResult.match)
                {
                    if (transferInfo.TransferMode == TransferMode.cut)
                    {
                        TransferUtils.RemoveDirectory(transferInfo.Source);
                    }
                    Console.WriteLine("All file hashes matched. File Transfer completed successfully!");  
                }
                else if (result == HashTransferResult.mismatch)
                {
                    Console.WriteLine("There are file hashes that did not match.");
                }
                else 
                {
                    throw new Exception();
                }
        });
    }
}

