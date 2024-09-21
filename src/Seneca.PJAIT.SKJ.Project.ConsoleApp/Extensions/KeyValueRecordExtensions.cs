using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Extensions;

public static class KeyValueRecordExtensions
{
    public static Pair ToPair(this KeyValueRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        // TODO: handle invalid values.
        return new Pair(int.Parse(record.Key), int.Parse(record.Value));
    }
}
