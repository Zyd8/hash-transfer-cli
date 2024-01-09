﻿
using CommandLine;

class HashTransferService
{
    private static void DoTransferOperation(SourceDestinationInfo SourceDestinationInfo)
    {
        TransferMode transferMode = SourceDestinationInfo.TransferMode;
        if (transferMode == TransferMode.copy)
        {
            if (!SourceDestinationInfo.IsSourceFile)
            {
                SourceDestination.CopyDirectory(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
                return;
            }
            File.Copy(SourceDestinationInfo.Source, SourceDestinationInfo.Destination, true);
            return;

        }
        else if (transferMode == TransferMode.cut)
        {
            if (!SourceDestinationInfo.IsSourceFile)
            {
                Directory.Move(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
                return;
            }
            File.Move(SourceDestinationInfo.Source, SourceDestinationInfo.Destination);
            return;

        }
        else
        {
            throw new ArgumentException();
        }

    }

    private static List<FileInfo> GetFileInfoList(string directoryPath, HashType hashType)
    {
        List<FileInfo> fileInfoList = new List<FileInfo>();
        int fileInfoCtr = 0;

        if (!Directory.Exists(directoryPath))
        {
            FileInfo fileInfoObj = new FileInfo(directoryPath, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);

            foreach (FileInfo fileInfo in fileInfoList)
            {
                fileInfo.HashValue = Hasher.GetFileHash(fileInfo.FilePath, hashType);
            }

            return fileInfoList;
        }

        List<string> filePaths = SourceDestination.TraverseDirectories(directoryPath);

        foreach (var path in filePaths)
        {
            FileInfo fileInfoObj = new FileInfo(path, fileInfoCtr += 1);
            fileInfoList.Add(fileInfoObj);
        }

        foreach (FileInfo fileInfo in fileInfoList)
        {
            fileInfo.HashValue = Hasher.GetFileHash(fileInfo.FilePath, hashType);
        }

        return fileInfoList;
    }
    
    private static HashTransferResult CheckHashMismatch(List<FileInfo> sourceFileInfoList, List<FileInfo> destFileInfoList, SourceDestinationInfo SourceDestinationInfo, HashType hashType, int recopyCtr, int recopyLimit)
    {
        if (recopyCtr > recopyLimit)
        {
            Console.WriteLine("Some file(s) seems to be continuously modified that hashes can't be matched");
            return HashTransferResult.mismatch;
        }

        var mismatchFilePairs = SourceDestination.FindHashMismatch(sourceFileInfoList, destFileInfoList);

        if (SourceDestinationInfo.TransferMode == TransferMode.copy && mismatchFilePairs.Count != 0)
        {
            Console.WriteLine("There are file hashes that did not match");
            Console.WriteLine($"Attempting to recopy mismatched files...{recopyCtr += 1}");
            SourceDestination.FileRecover(mismatchFilePairs);
            // Only refreshes file info of mismatch source and destination file hashes
            sourceFileInfoList = UpdateSourceInfoList(mismatchFilePairs, sourceFileInfoList, hashType);
            destFileInfoList = UpdateDestinationInfoList(mismatchFilePairs, destFileInfoList, hashType);
            // Recursive
            return CheckHashMismatch(sourceFileInfoList, destFileInfoList, SourceDestinationInfo, hashType, recopyCtr, recopyLimit);
        }
        else if (SourceDestinationInfo.TransferMode == TransferMode.cut && mismatchFilePairs.Count != 0)
        {
            return HashTransferResult.mismatch;
        }
        else
        {
            return HashTransferResult.match;
        }
    }

    private static List<FileInfo> UpdateSourceInfoList(List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)> mismatchFilePairs, List<FileInfo> sourceFileInfoList, HashType hashType)
    {
        foreach (var pair in mismatchFilePairs)
        {
            int mismatchKey = pair.SourceFileInfo.Key;

            FileInfo sourceFileInfoToUpdate = sourceFileInfoList.Find(info => info.Key == mismatchKey)!;

            if (sourceFileInfoToUpdate != null)
            {
                sourceFileInfoToUpdate.HashValue = Hasher.GetFileHash(sourceFileInfoToUpdate.FilePath, hashType);
            }
        }

        return sourceFileInfoList;
    }

    private static List<FileInfo> UpdateDestinationInfoList(List<(FileInfo SourceFileInfo, FileInfo DestFileInfo)> mismatchFilePairs, List<FileInfo> destFileInfoList, HashType hashType)
    {
        foreach (var pair in mismatchFilePairs)
        {
            int mismatchKey = pair.DestFileInfo.Key;

            FileInfo destFileInfoToUpdate = destFileInfoList.Find(info => info.Key == mismatchKey)!;

            if (destFileInfoToUpdate != null)
            {
                destFileInfoToUpdate.HashValue = Hasher.GetFileHash(destFileInfoToUpdate.FilePath, hashType);
            }
        }

        return destFileInfoList;
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

                SourceDestinationInfo SourceDestinationInfo = new(fullSourcePath, fullDestPath, transferMode);

                Console.WriteLine("Fetching source file hashes...");
                List<FileInfo> sourceFileInfoList = GetFileInfoList(SourceDestinationInfo.Source, hashType);

                Console.WriteLine("Transferring files...");
                DoTransferOperation(SourceDestinationInfo);

                Console.WriteLine("Fetching destination file hashes...");
                List<FileInfo> destFileInfoList = GetFileInfoList(SourceDestinationInfo.Destination, hashType);

                Console.WriteLine("Comparing file hashes...");
                int recopyCtr = 0; int recopyLimit = 10;
                HashTransferResult result = CheckHashMismatch(sourceFileInfoList, destFileInfoList, SourceDestinationInfo, hashType, recopyCtr, recopyLimit);

                if (result == HashTransferResult.match)
                {
                    Console.WriteLine("All file hashes matched. File Transfer completed successfully!");
                    Environment.Exit(0);
                }
                else if (result == HashTransferResult.mismatch)
                {
                    Console.WriteLine("There are file hashes that did not match");
                    Environment.Exit(0);
                }
                else 
                {
                    throw new Exception();
                }
        });
    }
}

