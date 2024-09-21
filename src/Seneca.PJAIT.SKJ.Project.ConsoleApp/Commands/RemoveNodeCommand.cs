namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

internal class RemoveNodeCommand(Node nodeToRemove)
    : Command(CommandName, nodeToRemove.ToString())
{
    public const string CommandName = "remove-node";
}
