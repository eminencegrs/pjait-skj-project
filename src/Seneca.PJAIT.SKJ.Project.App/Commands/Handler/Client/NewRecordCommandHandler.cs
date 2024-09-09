using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Storage;

namespace Seneca.PJAIT.SKJ.Project.Commands.Handler.Client;

public class NewRecordCommandHandler : CommandHandler
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
