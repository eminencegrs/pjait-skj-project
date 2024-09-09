using Seneca.PJAIT.SKJ.Project.Arguments;
using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Internal;

public class RemoveNodeCommandHandler : CommandHandler
{
    public static readonly string OperationName = RemoveNodeCommand.CommandName;

    private readonly NodeRegistry nodeRegistry;

    public RemoveNodeCommandHandler(NodeRegistry nodeRegistry)
    {
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return WithRequiredArgument(command, (arg) =>
        {
            Node node = Node.Parse(arg);
            this.nodeRegistry.RemoveNode(node);
            return Responses.Ok;
        });
    }
}
