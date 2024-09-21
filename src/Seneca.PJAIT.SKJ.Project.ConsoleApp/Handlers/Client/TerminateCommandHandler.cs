using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

internal class TerminateCommandHandler(INodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "terminate";

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        var removeNodeCommand = new RemoveNodeCommand(nodeRegistry.Self);

        foreach (var node in nodeRegistry.ConnectedNodes)
        {
            nodeRegistry.SendMessageToNode(node, removeNodeCommand.Serialize());
        }

        return Responses.Ok;
    }
}
