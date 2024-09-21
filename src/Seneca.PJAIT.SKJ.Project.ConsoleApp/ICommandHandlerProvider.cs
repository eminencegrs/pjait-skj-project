using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public interface ICommandHandlerProvider
{
    bool TryGetHandler(string operationName, out CommandHandlerBase? commandHandler);
}
