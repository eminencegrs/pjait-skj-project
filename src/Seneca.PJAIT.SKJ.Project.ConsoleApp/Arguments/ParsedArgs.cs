namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Arguments;

public class ParsedArgs
{
    public int TcpPort { get; set; }
    public KeyValueRecord Record { get; set; }
    public List<Node> Connect { get; } = [];

    public override string ToString()
    {
        return $"ParsedArgs{{tcpPort={this.TcpPort}, record={this.Record}, connect={string.Join(", ", this.Connect)}}}";
    }

    public void AddConnectNode(Node node)
    {
        this.Connect.Add(node);
    }
}
