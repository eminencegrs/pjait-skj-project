using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

internal class CommandHandlerProvider : ICommandHandlerProvider
{
    private readonly Lazy<Dictionary<string, CommandHandlerBase>> commandHandlers;

    public CommandHandlerProvider(Func<IEnumerable<CommandHandlerBase>> getCommandHandlers)
    {
        this.commandHandlers = new Lazy<Dictionary<string, CommandHandlerBase>>(
            () => getCommandHandlers().ToDictionary(h => h.GetOperationName(), h => h),
            isThreadSafe: true);
    }

    public bool TryGetHandler(string operationName, out CommandHandlerBase? commandHandler)
    {
        if (this.commandHandlers.Value.TryGetValue(operationName, out var handler))
        {
            commandHandler = handler;
            return true;
        }

        commandHandler = null;
        return false;
    }
}
