using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class SetValueCommandHandler : CommandHandlerBase
{
    public static readonly string OperationName = "set-value";

    private readonly SimpleKeyValueStorage kvStorage;
    private readonly NodeRegistry nodeRegistry;

    public SetValueCommandHandler(SimpleKeyValueStorage kvStorage, NodeRegistry nodeRegistry)
    {
        this.kvStorage = kvStorage;
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var inputPair = Pair.Parse(arg);
            var maybePair = this.kvStorage.SetNewValue(inputPair.Key, inputPair.Value);

            var commandArgument = new ForwardCommandArgument(this.GetOperationName(), sessionId, this.nodeRegistry.Self, arg);
            var forwardCommand = new ForwardCommand(commandArgument);

            var responses = this.nodeRegistry.SendMessageToNodesAndGatherResponses(forwardCommand.Serialize(), commandArgument.SessionId);

            if (maybePair != null)
            {
                Console.WriteLine($"[SetValueCommandHandler] Set pair [{inputPair}]");
                return Responses.Ok;
            }
            else
            {
                var resp = responses.Values.FirstOrDefault(r => r == Responses.Ok) ?? Responses.Error;
                var connectedNodesUpdatedValue = resp == Responses.Ok;
                Console.WriteLine($"[SetValueCommandHandler] Key [{inputPair.Key}] not exist on this node. Connected nodes updated value = [{connectedNodesUpdatedValue}]");
                return resp;
            }
        });
    }
}
