using System.CommandLine;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public static class CommandLineOptions
{
    private static readonly Option<int> TcpPortOption = new(
        name: "--tcpport",
        description: "The TCP port number")
    {
        IsRequired = true
    };

    private static readonly Option<string> RecordOption = new(
        name: "--record",
        description: "The record to store")
    {
        IsRequired = true
    };

    private static readonly Option<IEnumerable<string>> ConnectedNodesOption = new(
        name: "--connect",
        description: "List of connected nodes")
    {
        AllowMultipleArgumentsPerToken = true,
        IsRequired = false
    };

    public static Option<int> GetTcpPortOption() => TcpPortOption;
    public static Option<string> GetRecordOption() => RecordOption;
    public static Option<IEnumerable<string>> GetConnectedNodesOption() => ConnectedNodesOption;
}
