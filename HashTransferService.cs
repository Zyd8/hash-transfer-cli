
using CommandLine;

class HashTransferService
{
    private static HashTransferResult CheckHashMismatch(FileHashManager fileHashManager, TransferInfo TransferInfo, HashType hashType, int recopyCtr, int recopyLimit)
    {
        if (recopyCtr > recopyLimit)
        {
            Console.WriteLine("Some file(s) seems to be continuously modified that hashes can't be matched");
            return HashTransferResult.mismatch;
        }

        fileHashManager.FindHashMismatch();

        if (TransferInfo.TransferMode == TransferMode.copy && fileHashManager.mismatchHashInfoPair.Count != 0)
        {
            Console.WriteLine("There are file hashes that did not match");
            Console.WriteLine($"Attempting to recopy mismatched files...{recopyCtr += 1}");
            fileHashManager.MismatchHashResolver();
            // Only refreshes file info of mismatch source and destination file hashes
            fileHashManager.UpdateHashInfoList(hashType);
            // Recursive
            return CheckHashMismatch(fileHashManager, TransferInfo, hashType, recopyCtr, recopyLimit);
        }
        else if (TransferInfo.TransferMode == TransferMode.cut && fileHashManager.mismatchHashInfoPair.Count != 0)
        {
            // MismatchHashResolver would not work with 'cut' transfer mode
            return HashTransferResult.mismatch;
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
                string currentDirectory = Directory.GetCurrentDirectory();

                string fullSourcePath = Path.GetFullPath(Path.Combine(currentDirectory, Input.ParseLeadingSlash(options.InputSourcePath)));

                string fullDestPath = Path.GetFullPath(Path.Combine(currentDirectory, Input.ParseLeadingSlash(options.InputDestinationPath), Path.GetFileName(options.InputSourcePath)));

                TransferMode transferMode = Input.ParseTransferMode(options.InputTransferMode);

                HashType hashType = Input.ParseHashType(options.InputHashType);

                TransferInfo TransferInfo = new(fullSourcePath, fullDestPath, transferMode);

                FileHashManager fileHashManager = new();

                Console.WriteLine("Fetching source file hashes...");
                fileHashManager.GetSourceInfoList(TransferInfo.Source, hashType);

                Console.WriteLine("Transferring files...");
                TransferUtils.DoTransferOperation(TransferInfo);

                Console.WriteLine("Fetching destination file hashes...");
                fileHashManager.GetDestinationInfoList(TransferInfo.Destination, hashType);

                Console.WriteLine("Comparing file hashes...");
                int recopyCtr = 0; int recopyLimit = 30;

                HashTransferResult result = CheckHashMismatch(fileHashManager, TransferInfo, hashType, recopyCtr, recopyLimit);

                if (result == HashTransferResult.match)
                {
                    Console.WriteLine("All file hashes matched. File Transfer completed successfully!");  
                }
                else if (result == HashTransferResult.mismatch)
                {
                    Console.WriteLine("There are file hashes that did not match");
                }
                else 
                {
                    throw new Exception();
                }
        });
    }
}

