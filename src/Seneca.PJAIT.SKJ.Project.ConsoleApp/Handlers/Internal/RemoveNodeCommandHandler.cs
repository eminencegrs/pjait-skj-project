using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

internal class RemoveNodeCommandHandler(INodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = RemoveNodeCommand.CommandName;

    public override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var node = Node.Parse(arg);
            nodeRegistry.RemoveConnectedNode(node);
            return Responses.Ok;
        });
    }
}
