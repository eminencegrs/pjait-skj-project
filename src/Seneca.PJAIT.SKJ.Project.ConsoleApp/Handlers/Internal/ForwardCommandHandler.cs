using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;

public class ForwardCommandHandler(
    Dictionary<string, CommandHandlerBase> commandHandlers, NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = ForwardCommand.CommandName;

    private readonly Dictionary<string, CommandHandlerBase> cmdHandlers = new(commandHandlers);

    protected override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var argument = ForwardCommandArgument.Parse(arg);
            if (argument.ClientOperation == OperationName)
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] ForwardCommandHandler cannot process the command [{OperationName}]");
                return Responses.Error;
            }

            if (!this.cmdHandlers.TryGetValue(argument.ClientOperation, out CommandHandlerBase? commandHandler))
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] Could not find a command handler for the operation [{argument.ClientOperation}]");
                return Responses.Error;
            }

            if (nodeRegistry.ProcessedSessionId(argument.SessionId))
            {
                Console.WriteLine(
                    $"[{nameof(ForwardCommandHandler)}] This node has already processed the operation [{argument.ClientOperation}] with sessionId [{argument.SessionId}]");
                nodeRegistry.AddVisitedNode(argument.From, argument.SessionId);
                return Responses.Error;
            }

            nodeRegistry.AddVisitedNode(argument.From, argument.SessionId);

            var clientCommand = new Command(argument.ClientOperation, argument.Argument);
            return commandHandler.Handle(clientCommand, argument.SessionId);
        });
    }
}
