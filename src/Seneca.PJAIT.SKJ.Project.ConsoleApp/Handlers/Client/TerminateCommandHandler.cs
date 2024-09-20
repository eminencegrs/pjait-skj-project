using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class TerminateCommandHandler : CommandHandlerBase
{
    public static readonly string OperationName = "terminate";

    private readonly NodeRegistry nodeRegistry;

    public TerminateCommandHandler(NodeRegistry nodeRegistry)
    {
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        var removeNodeCmd = new RemoveNodeCommand(this.nodeRegistry.Self);

        foreach (var node in this.nodeRegistry.Nodes)
        {
            this.nodeRegistry.SendMessageToNode(node, removeNodeCmd.Serialize());
        }

        return Responses.Ok;
    }
}
