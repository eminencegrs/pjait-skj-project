namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class ForwardCommand(ForwardCommandArgument fwdCommArg) : Command(CommandName, fwdCommArg.Serialize())
{
    public static readonly string CommandName = "forward";
}
