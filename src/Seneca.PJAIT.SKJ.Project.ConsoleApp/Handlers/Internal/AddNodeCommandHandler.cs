using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

public class AddNodeCommandHandler : CommandHandlerBase
{
    public static readonly string OperationName = AddNodeCommand.CommandName;

    private readonly NodeRegistry nodeRegistry;

    public AddNodeCommandHandler(NodeRegistry nodeRegistry)
    {
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            Node newNode = Node.Parse(arg);
            this.nodeRegistry.AddNode(newNode);
            return this.nodeRegistry.Self.ToString();
        });
    }
}
