using Seneca.PJAIT.SKJ.Project.Commands.Models;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler;

public abstract class CommandHandler
{
    public abstract string Handle(Command command, string sessionId);

    public abstract string GetOperationName();

    protected string WithRequiredArgument(Command command, Func<string, string> fn)
    {
        if (command.Argument != null)
        {
            string arg = command.Argument;
            return fn(arg);
        }
        else
        {
            Console.WriteLine($"[CommandHandler] Argument for operation [{this.GetOperationName()}] not found");
            return Responses.Error;
        }
    }
}
