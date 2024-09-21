using System.CommandLine;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class AppProcessor(IDatabaseNode databaseNode)
{
    public async Task Run(string[] args)
    {
        var rootCommand = RootCommandFactory.CreateRootCommand(
            databaseNode,
            (dbNode, tcpPort, record, connectedNodes) => dbNode.Run(tcpPort, record, connectedNodes));

        await rootCommand.InvokeAsync(args);
    }
}
