namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class RemoveNodeCommand(Node nodeToRemove)
    : Command(CommandName, nodeToRemove.ToString())
{
    public const string CommandName = "remove-node";
}
