using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class TerminateCommandHandler(NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "terminate";

    protected override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        var removeNodeCommand = new RemoveNodeCommand(nodeRegistry.Self);

        foreach (var node in nodeRegistry.Nodes)
        {
            nodeRegistry.SendMessageToNode(node, removeNodeCommand.Serialize());
        }

        return Responses.Ok;
    }
}
