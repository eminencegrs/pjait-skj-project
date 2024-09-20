namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

internal class AddNodeCommand(Node nodeToAdd)
    : Command(CommandName, nodeToAdd.ToString())
{
    public const string CommandName = "add-node";
}
