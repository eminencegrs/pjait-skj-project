using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;

internal class NewRecordCommandHandler(IKeyValueStorage keyValueStorage)
    : CommandHandlerBase
{
    public static readonly string OperationName = "new-record";

    public override string GetOperationName() => OperationName;

    public override string? Handle(Command command, string sessionId)
    {
        return this.WithRequiredArgument(command, arg =>
        {
            var pair = Pair.Parse(arg);
            keyValueStorage.SetKeyValue(pair);
            Console.WriteLine($"[{nameof(NewRecordCommandHandler)}] Saved the new pair: {pair}");
            return Responses.Ok;
        });
    }
}
