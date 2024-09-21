using System.CommandLine;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public static class RootCommandFactory
{
    public static RootCommand CreateRootCommand(
        IDatabaseNode databaseNode,
        Func<IDatabaseNode, int, KeyValueRecord, IReadOnlyCollection<Node>, Task> handler)
    {
        var tcpPortOption = CommandLineOptions.GetTcpPortOption();
        var recordOption = CommandLineOptions.GetRecordOption();
        var connectedNodesOption = CommandLineOptions.GetConnectedNodesOption();

        var rootCommand = new RootCommand("Seneca.PJAIT.SKJ.Project.ConsoleApp")
        {
            tcpPortOption,
            recordOption,
            connectedNodesOption,
        };

        rootCommand.SetHandler(
            async (tcpPort, record, nodes) => { await handler(databaseNode, tcpPort, record, nodes); },
            tcpPortOption,
            new KeyValueRecordBinder(recordOption),
            new NodeBinder(connectedNodesOption));

        return rootCommand;
    }
}
