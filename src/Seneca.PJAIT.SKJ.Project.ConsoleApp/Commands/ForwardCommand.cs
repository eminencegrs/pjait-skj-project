namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class ForwardCommand(ForwardCommandArgument forwardCommandArgument)
    : Command(CommandName, forwardCommandArgument.Serialize())
{
    public const string CommandName = "forward";
}
