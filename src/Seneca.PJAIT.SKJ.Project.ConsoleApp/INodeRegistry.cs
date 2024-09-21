namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public interface INodeRegistry
{
    Node Self { get; }
    HashSet<Node> ConnectedNodes { get; }
    void Initialize(Node node);
    void AddConnectedNode(Node connectedNode);
    void RemoveConnectedNode(Node connectedNode);
    string? SendMessageToNode(Node destination, string message);
    string? SendMessageToNodesUntilFirstReply(string message, string sessionId);
    Dictionary<Node, string> SendMessageToNodesAndGatherResponses(string message, string sessionId);
    void AddVisitedNode(Node visitedNode, string sessionId);
    bool ProcessedSessionId(string sessionId);
}
