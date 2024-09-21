namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

internal class ForwardCommand(ForwardCommandArgument forwardCommandArgument)
    : Command(CommandName, forwardCommandArgument.Serialize())
{
    public const string CommandName = "forward";
}
