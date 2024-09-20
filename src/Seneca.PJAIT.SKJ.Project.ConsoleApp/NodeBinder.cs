using System.CommandLine;
using System.CommandLine.Binding;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class NodeBinder(Option<IEnumerable<string>> nodeOption) : BinderBase<IReadOnlyCollection<Node>>
{
    protected override IReadOnlyCollection<Node> GetBoundValue(BindingContext bindingContext)
    {
        var nodeOptionValues = bindingContext.ParseResult.GetValueForOption(nodeOption);
        return (nodeOptionValues is null ? [] : nodeOptionValues.Select(Node.Parse)).ToList();
    }
}
