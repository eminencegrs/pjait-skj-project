using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Internal;

public class ForwardCommandHandler : CommandHandler
{
    public static readonly string OperationName = ForwardCommand.CommandName;

    private readonly Dictionary<string, CommandHandler> cmdHandlers;
    private readonly NodeRegistry nodeRegistry;

    public ForwardCommandHandler(Dictionary<string, CommandHandler> commandHandlers, NodeRegistry nodeRegistry)
    {
        this.cmdHandlers = new Dictionary<string, CommandHandler>(commandHandlers);
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return WithRequiredArgument(command, (arg) =>
        {
            ForwardCommandArgument internalArg = ForwardCommandArgument.Parse(arg);
            if (internalArg.ClientOperation == OperationName)
            {
                Console.WriteLine($"[ForwardCommandHandler] ForwardCommandHandler can't process command [{OperationName}]");
                return Responses.Error;
            }

            if (!this.cmdHandlers.TryGetValue(internalArg.ClientOperation, out CommandHandler commandHandler))
            {
                Console.WriteLine($"[ForwardCommandHandler] Can't find command handler for operation [{internalArg.ClientOperation}]");
                return Responses.Error;
            }

            if (this.nodeRegistry.ProcessedSessionId(internalArg.SessionId))
            {
                Console.WriteLine($"[ForwardCommandHandler] This node processed operation [{internalArg.ClientOperation}] with sessionId [{internalArg.SessionId}] already");
                this.nodeRegistry.AddVisitedNode(internalArg.From, internalArg.SessionId);
                return Responses.Error;
            }

            this.nodeRegistry.AddVisitedNode(internalArg.From, internalArg.SessionId);

            Command clientCommand = new Command(internalArg.ClientOperation, internalArg.Argument);
            return commandHandler.Handle(clientCommand, internalArg.SessionId);
        });
    }
}
