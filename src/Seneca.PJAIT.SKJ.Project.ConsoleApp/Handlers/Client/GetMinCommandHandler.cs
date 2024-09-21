using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class GetMinCommandHandler(IKeyValueStorage keyValueStorage, NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "get-min";

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

        var minPair = valuesToCompare.MinBy(x => x.Value) ?? ownPair;

        Console.WriteLine($"[{nameof(GetMinCommandHandler)}] The MIN value is: [{minPair}]");

        return minPair.ToString();
    }
}
