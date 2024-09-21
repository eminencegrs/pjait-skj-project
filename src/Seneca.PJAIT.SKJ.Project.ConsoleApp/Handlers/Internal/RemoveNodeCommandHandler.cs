using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

public class RemoveNodeCommandHandler : CommandHandlerBase
{
    public static readonly string OperationName = RemoveNodeCommand.CommandName;

    private readonly NodeRegistry nodeRegistry;

    public RemoveNodeCommandHandler(NodeRegistry nodeRegistry)
    {
        this.nodeRegistry = nodeRegistry;
    }

    protected override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var node = Node.Parse(arg);
            this.nodeRegistry.RemoveNode(node);
            return Responses.Ok;
        });
    }
}
