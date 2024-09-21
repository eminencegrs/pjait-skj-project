using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

public class AddNodeCommandHandler(NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = AddNodeCommand.CommandName;

    protected override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var newNode = Node.Parse(arg);
            nodeRegistry.AddNode(newNode);
            return nodeRegistry.Self.ToString();
        });
    }
}
