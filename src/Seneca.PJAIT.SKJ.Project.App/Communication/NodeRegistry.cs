using System.Collections.Concurrent;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.Arguments;
using Seneca.PJAIT.SKJ.Project.Commands.Models;

namespace Seneca.PJAIT.SKJ.Project.Communication;

public class NodeRegistry
{
    private readonly ConcurrentDictionary<string, HashSet<Node>> sessionIdToVisitedNodes = new();

    public NodeRegistry(HashSet<Node> nodes, Node self)
    {
        this.Nodes = nodes;
        this.Self = self;
    }

    public HashSet<Node> Nodes { get; }
    public Node Self { get; }

    public void AddNode(Node newNode)
    {
        this.Nodes.Add(newNode);
        Console.WriteLine(
            $"[NodeRegistry] Added a new node [{newNode}] to registry. " +
            $"Current registry: [{NodeUtils.NodesToString(this.Nodes)}]");
    }

    public void RemoveNode(Node node)
    {
        this.Nodes.Remove(node);
        Console.WriteLine(
            $"[NodeRegistry] Removed the node [{node}] from registry. " +
            $"Current registry: [{NodeUtils.NodesToString(this.Nodes)}]");
    }

    public string SendMessageToNode(Node target, string msgToSend)
    {
        try
        {
            using var clientSocket = new TcpClient(target.Host, target.Port);
            using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(clientSocket.GetStream());

            Console.WriteLine($"[NodeRegistry] Sending the message [{msgToSend}] to the node [{target}]");

            writer.WriteLine(msgToSend);

            return reader.ReadLine();
        }
        catch (IOException e)
        {
            Console.WriteLine($"[NodeRegistry] Error while trying to send the message [{msgToSend}] to node [{target}]: {e.Message}");
            return Responses.Error;
        }
    }

    public string SendMessageToNodesUntilFirstReplied(string msgToSend, string sessionId)
    {
        var notVisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
        return this.SendUntilFirstRepliedRecursive(msgToSend, sessionId, notVisitedNodes);
    }

    private string SendUntilFirstRepliedRecursive(string msgToSend, string sessionId, HashSet<Node> notVisitedNodes)
    {
        Console.WriteLine($"[NodeRegistry] Nonvisited nodes left: [{NodeUtils.NodesToString(notVisitedNodes)}]");
        var maybeNotVisitedNode = notVisitedNodes.FirstOrDefault();
        if (maybeNotVisitedNode != null)
        {
            this.AddVisitedNode(maybeNotVisitedNode, sessionId);
            var response = this.SendMessageToNode(maybeNotVisitedNode, msgToSend);
            if (response != Responses.Error)
            {
                Console.WriteLine($"[NodeRegistry] Successful response [{response}] from node [{maybeNotVisitedNode}]");
                this.ClearVisitedNodes(sessionId);
                return response;
            }
            else
            {
                var newNotVisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
                return this.SendUntilFirstRepliedRecursive(msgToSend, sessionId, newNotVisitedNodes);
            }
        }
        else
        {
            Console.WriteLine("[NodeRegistry] No more unvisited nodes left. Returning ERROR");
            return Responses.Error;
        }
    }

    public Dictionary<Node, string> SendMessageToNodesAndGatherResponses(string msgToSend, string sessionId)
    {
        var notVisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
        return this.SendToAllNodesRecursive(msgToSend, sessionId, notVisitedNodes, new Dictionary<Node, string>());
    }

    private Dictionary<Node, string> SendToAllNodesRecursive(string msgToSend, string sessionId, HashSet<Node> notVisitedNodes, Dictionary<Node, string> accumulator)
    {
        Console.WriteLine($"[NodeRegistry] Nonvisited nodes left: [{NodeUtils.NodesToString(notVisitedNodes)}]");
        var maybeNotVisitedNode = notVisitedNodes.FirstOrDefault();
        if (maybeNotVisitedNode != null)
        {
            this.AddVisitedNode(maybeNotVisitedNode, sessionId);
            var response = this.SendMessageToNode(maybeNotVisitedNode, msgToSend);
            Console.WriteLine($"[NodeRegistry] Response [{response}] from node [{maybeNotVisitedNode}]");
            accumulator[maybeNotVisitedNode] = response;

            var newNotVisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
            return this.SendToAllNodesRecursive(msgToSend, sessionId, newNotVisitedNodes, accumulator);
        }
        else
        {
            Console.WriteLine($"[NodeRegistry] No more unvisited nodes left. Returned responses: [{NodeUtils.MapToString(accumulator)}]");
            return accumulator;
        }
    }

    public void AddVisitedNode(Node visitedNode, string sessionId)
    {
        Console.WriteLine($"[NodeRegistry] Trying to add visited node [{visitedNode}]");
        this.sessionIdToVisitedNodes.AddOrUpdate(sessionId, new HashSet<Node> { visitedNode },
            (key, visitedNodes) =>
            {
                visitedNodes.Add(visitedNode);
                Console.WriteLine($"[NodeRegistry] Added visited node [{visitedNode}] to session [{sessionId}]. Current list of visited nodes: [{NodeUtils.NodesToString(visitedNodes)}]");
                return visitedNodes;
            });
    }

    private void ClearVisitedNodes(string sessionId)
    {
        Console.WriteLine($"[NodeRegistry] Trying to clear visited nodes for session [{sessionId}]");
        this.sessionIdToVisitedNodes.TryRemove(sessionId, out _);
    }

    private HashSet<Node> GetUnvisitedNodesForSession(string sessionId)
    {
        var visitedNodes = this.sessionIdToVisitedNodes.GetValueOrDefault(sessionId, new HashSet<Node>());
        return this.Nodes.Where(node => !visitedNodes.Contains(node)).ToHashSet();
    }

    public bool ProcessedSessionId(string sessionId)
    {
        return this.sessionIdToVisitedNodes.ContainsKey(sessionId);
    }
}
