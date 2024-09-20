using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;

public abstract class CommandHandlerBase
{
    public abstract string? Handle(Command command, string sessionId);

    protected abstract string GetOperationName();

    protected string? WithRequiredArgument(Command command, Func<string, string?> function)
    {
        if (string.IsNullOrEmpty(command.Argument))
        {
            Console.WriteLine(
                $"{nameof(CommandHandlerBase)}: The command argument must not be null or empty " +
                $"for the operation '{this.GetOperationName()}'.");
            return Responses.Error;
        }

        var argument = command.Argument;
        return function(argument);
    }
}
