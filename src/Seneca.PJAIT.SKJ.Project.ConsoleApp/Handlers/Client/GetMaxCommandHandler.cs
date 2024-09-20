using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class GetMaxCommandHandler(KeyValueStorage keyValueStorage, NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "get-max";

    protected override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        var ownPair = keyValueStorage.GetPair();

        var argument = new ForwardCommandArgument(this.GetOperationName(), sessionId, nodeRegistry.Self);
        var forwardCommand = new ForwardCommand(argument);

        var responses = nodeRegistry.SendMessageToNodesAndGatherResponses(
            forwardCommand.Serialize(), argument.SessionId);

        var valuesToCompare = responses.Values
            .Where(respValue => respValue != Responses.Error)
            .Select(Pair.Parse)
            .ToList();

        valuesToCompare.Add(ownPair);

        var maxPair = valuesToCompare.MaxBy(x => x.Value) ?? ownPair;

        Console.WriteLine($"[{nameof(GetMaxCommandHandler)}] The MAX value is: [{maxPair}]");

        return maxPair.ToString();
    }
}
