using Seneca.PJAIT.SKJ.Project.ConsoleApp.Arguments;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands.Models;

public class RemoveNodeCommand : Command
{
    public const string CommandName = "remove-node";

    public RemoveNodeCommand(Node nodeToRemove)
        : base(CommandName, nodeToRemove.ToString())
    {
    }
}
