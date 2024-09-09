using Seneca.PJAIT.SKJ.Project.Arguments;
using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;
using Seneca.PJAIT.SKJ.Project.Storage;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Client;

public class GetMinCommandHandler : CommandHandler
{
    public static readonly string OperationName = "get-min";

    private readonly SimpleKeyValueStorage kvStorage;
    private readonly NodeRegistry nodeRegistry;

    public GetMinCommandHandler(SimpleKeyValueStorage kvStorage, NodeRegistry nodeRegistry)
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

        Pair minPair = valuesToCompare.MinBy(x => x.Value) ?? ownPair;

        Console.WriteLine($"[GetMinCommandHandler] Min value is: [{minPair}]");

        return minPair.ToString();
    }
}
