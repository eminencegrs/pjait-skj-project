using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

public class NewRecordCommandHandler : CommandHandlerBase
{
    public static readonly string OperationName = "new-record";

    private readonly SimpleKeyValueStorage kvStorage;

    public NewRecordCommandHandler(SimpleKeyValueStorage kvStorage)
    {
        this.kvStorage = kvStorage;
    }

    public override string GetOperationName() => OperationName;

    public override string Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var pair = Pair.Parse(arg);
            this.kvStorage.NewPair(pair);
            Console.WriteLine($"[NewRecordCommandHandler] Saved new pair: {pair}");
            return Responses.Ok;
        });
    }
}
