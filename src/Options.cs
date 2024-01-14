using CommandLine;

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