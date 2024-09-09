using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;
using Seneca.PJAIT.SKJ.Project.Storage;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Client;

public class FindKeyCommandHandler : CommandHandler
{
    public static readonly string OperationName = "find-key";

    private readonly SimpleKeyValueStorage kvStorage;
    private readonly NodeRegistry nodeRegistry;

    public FindKeyCommandHandler(SimpleKeyValueStorage kvStorage, NodeRegistry nodeRegistry)
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
                Console.WriteLine($"[FindKeyCommandHandler] Found pair for key [{key}]");
                return this.nodeRegistry.Self.ToString();
            }
            else
            {
                Console.WriteLine($"[FindKeyCommandHandler] Key [{key}] not exist on this node");
                var fwdCommArg = new ForwardCommandArgument(this.GetOperationName(), sessionId, this.nodeRegistry.Self, arg);
                var forwardCommand = new ForwardCommand(fwdCommArg);
                return this.nodeRegistry.SendMessageToNodesUntilFirstReplied(forwardCommand.Serialize(), sessionId);
            }
        });
    }
}
