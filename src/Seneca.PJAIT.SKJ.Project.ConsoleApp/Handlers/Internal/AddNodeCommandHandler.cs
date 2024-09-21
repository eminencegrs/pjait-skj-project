using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

internal class AddNodeCommandHandler(INodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = AddNodeCommand.CommandName;

    public override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var newNode = Node.Parse(arg);
            nodeRegistry.AddConnectedNode(newNode);
            return nodeRegistry.Self.ToString();
        });
    }
}
