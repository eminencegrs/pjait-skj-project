namespace Seneca.PJAIT.SKJ.Project.Commands.Models;

public class ForwardCommand : Command
{
    public static readonly string CommandName = "forward";

    public ForwardCommand(ForwardCommandArgument fwdCommArg) 
        : base(CommandName, fwdCommArg.Serialize())
    {
    }
}