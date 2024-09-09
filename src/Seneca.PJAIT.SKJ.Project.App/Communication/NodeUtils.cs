using Seneca.PJAIT.SKJ.Project.Arguments;

namespace Seneca.PJAIT.SKJ.Project.Communication;

public static class NodeUtils
{
    public static string NodesToString(HashSet<Node> nodes)
    {
        return string.Join(",", nodes.Select(node => node.ToString()));
    }

    public static string MapToString(Dictionary<Node, string> map)
    {
        return "{" + string.Join(", ", map.Select(kvp => $"{kvp.Key}={kvp.Value}")) + "}";
    }
}
