namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Extensions;

internal static class NodeExtensions
{
    public static string ToSeparatedString(this IEnumerable<Node> nodes, string separator = ",") =>
        string.Join(separator, nodes.Select(node => node.ToString()));

    public static string ToSeparatedString(this Dictionary<Node, string> map, string separator = ", ") =>
        "{" + string.Join(separator, map.Select(kvp => $"{kvp.Key}={kvp.Value}")) + "}";
}
