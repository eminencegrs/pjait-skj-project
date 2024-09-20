namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class AddNodeCommand(Node nodeToAdd) : Command(CommandName, nodeToAdd.ToString())
{
    public static readonly string CommandName = "add-node";
}
