using System.CommandLine;
using System.CommandLine.Binding;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class KeyValueRecordBinder(Option<string> recordOption) : BinderBase<KeyValueRecord>
{
    protected override KeyValueRecord GetBoundValue(BindingContext bindingContext)
    {
        var recordOptionValue = bindingContext.ParseResult.GetValueForOption(recordOption);
        ArgumentException.ThrowIfNullOrEmpty(recordOptionValue);
        return KeyValueRecord.Parse(recordOptionValue);
    }
}
