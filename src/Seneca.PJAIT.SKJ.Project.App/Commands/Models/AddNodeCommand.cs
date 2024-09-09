using Seneca.PJAIT.SKJ.Project.Arguments;

namespace Seneca.PJAIT.SKJ.Project.Commands.Models;

public class AddNodeCommand : Command
{
    public static readonly string CommandName = "add-node";

    public AddNodeCommand(Node nodeToAdd)
        : base(CommandName, nodeToAdd.ToString())
    {
    }
}
