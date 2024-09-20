using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands.Models;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands.Handler.Client;

public class GetValueCommandHandler : CommandHandler
{
    public static readonly string OperationName = "get-value";

    private readonly SimpleKeyValueStorage kvStorage;
    private readonly NodeRegistry nodeRegistry;

    public GetValueCommandHandler(SimpleKeyValueStorage kvStorage, NodeRegistry nodeRegistry)
    {
        this.kvStorage = kvStorage;
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            int key = int.Parse(arg);
            var maybePair = this.kvStorage.GetValue(key);
            if (maybePair != null)
            {
                Console.WriteLine($"[GetValueCommandHandler] Found pair for key [{key}]");
                return maybePair.Value.ToString();
            }
            else
            {
                Console.WriteLine($"[GetValueCommandHandler] Key [{key}] not exist");

                var commandArgument = new ForwardCommandArgument(this.GetOperationName(), sessionId, this.nodeRegistry.Self, arg);
                var forwardCommand = new ForwardCommand(commandArgument);
                return this.nodeRegistry.SendMessageToNodesUntilFirstReplied(forwardCommand.Serialize(), sessionId);
            }
        });
    }
}
