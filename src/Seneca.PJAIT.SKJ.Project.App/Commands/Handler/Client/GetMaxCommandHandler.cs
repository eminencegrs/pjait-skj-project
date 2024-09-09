using Seneca.PJAIT.SKJ.Project.Arguments;
using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;
using Seneca.PJAIT.SKJ.Project.Storage;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Client;

public class GetMaxCommandHandler : CommandHandler
{
    public static readonly string OperationName = "get-max";

    private readonly SimpleKeyValueStorage kvStorage;
    private readonly NodeRegistry nodeRegistry;

    public GetMaxCommandHandler(SimpleKeyValueStorage kvStorage, NodeRegistry nodeRegistry)
    {
        this.kvStorage = kvStorage;
        this.nodeRegistry = nodeRegistry;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        Pair ownPair = this.kvStorage.GetPair();

        var forwardCommandArgument = new ForwardCommandArgument(GetOperationName(), sessionId, nodeRegistry.Self);
        var forwardCommand = new ForwardCommand(forwardCommandArgument);

        Dictionary<Node, string> responses = this.nodeRegistry.SendMessageToNodesAndGatherResponses(
            forwardCommand.Serialize(), forwardCommandArgument.SessionId);

        List<Pair> valuesToCompare = responses.Values
            .Where(respValue => respValue != Responses.Error)
            .Select(Pair.Parse)
            .ToList();

        valuesToCompare.Add(ownPair);

        Pair maxPair = valuesToCompare.MaxBy(x => x.Value) ?? ownPair;

        Console.WriteLine("[GetMaxCommandHandler] Max value is: [{0}]", maxPair);

        return maxPair.ToString();
    }
}
