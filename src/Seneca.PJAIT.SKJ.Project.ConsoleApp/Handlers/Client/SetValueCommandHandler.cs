using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class SetValueCommandHandler(KeyValueStorage keyValueStorage, NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "set-value";

    protected override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var inputPair = Pair.Parse(arg);
            var maybePair = keyValueStorage.SetNewValue(inputPair.Key, inputPair.Value);

            var argument = new ForwardCommandArgument(this.GetOperationName(), sessionId, nodeRegistry.Self, arg);
            var forwardCommand = new ForwardCommand(argument);

            var responses = nodeRegistry.SendMessageToNodesAndGatherResponses(forwardCommand.Serialize(), argument.SessionId);

            if (maybePair != null)
            {
                Console.WriteLine($"[{nameof(SetValueCommandHandler)}] Set the pair [{inputPair}]");
                return Responses.Ok;
            }

            var firstOkResponseOrError = responses.Values.FirstOrDefault(r => r == Responses.Ok) ?? Responses.Error;
            var connectedNodesUpdatedValue = firstOkResponseOrError == Responses.Ok;

            Console.WriteLine(
                $"[{nameof(SetValueCommandHandler)}] The key [{inputPair.Key}] does not exist on this node. " +
                $"Connected nodes updated value = [{connectedNodesUpdatedValue}]");

            return firstOkResponseOrError;
        });
    }
}
