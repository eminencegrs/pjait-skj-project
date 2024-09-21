using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

internal class ForwardCommandHandler(ICommandHandlerProvider provider, INodeRegistry registry)
    : CommandHandlerBase
{
    public static readonly string OperationName = ForwardCommand.CommandName;

    public override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var argument = ForwardCommandArgument.Parse(arg);
            if (argument.ClientOperation == OperationName)
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] Cannot process the command [{OperationName}]");
                return Responses.Error;
            }

            if (!provider.TryGetHandler(argument.ClientOperation, out var commandHandler))
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] Could not find a command handler " +
                    $"for the operation [{argument.ClientOperation}]");
                return Responses.Error;
            }

            if (registry.ProcessedSessionId(argument.SessionId))
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] This node has already processed the operation " +
                    $"[{argument.ClientOperation}] with sessionId [{argument.SessionId}]");

                registry.AddVisitedNode(argument.From, argument.SessionId);
                return Responses.Error;
            }

            registry.AddVisitedNode(argument.From, argument.SessionId);
            var clientCommand = new Command(argument.ClientOperation, argument.Argument);
            return commandHandler!.Handle(clientCommand, argument.SessionId);
        });
    }
}
