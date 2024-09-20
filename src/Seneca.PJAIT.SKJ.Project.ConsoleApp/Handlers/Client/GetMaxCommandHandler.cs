using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class GetMaxCommandHandler : CommandHandlerBase
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

        var forwardCommandArgument = new ForwardCommandArgument(this.GetOperationName(), sessionId, this.nodeRegistry.Self);
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
