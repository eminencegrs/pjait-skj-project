using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class FindKeyCommandHandler(IKeyValueStorage keyValueStorage, NodeRegistry nodeRegistry)
    : CommandHandlerBase
{
    public static readonly string OperationName = "find-key";

    protected override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var key = int.Parse(arg);
            var pair = keyValueStorage.GetValue(key);
            if (pair != null)
            {
                Console.WriteLine($"[FindKeyCommandHandler] Found pair for key [{key}]");
                return nodeRegistry.Self.ToString();
            }

            Console.WriteLine($"[{nameof(FindKeyCommandHandler)}] The key [{key}] does not exist on this node");

            var argument = new ForwardCommandArgument(this.GetOperationName(), sessionId, nodeRegistry.Self, arg);
            var forwardCommand = new ForwardCommand(argument);

            return nodeRegistry.SendMessageToNodesUntilFirstReply(forwardCommand.Serialize(), sessionId);
        });
    }
}
