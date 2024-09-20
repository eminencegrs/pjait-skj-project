using System.Collections.Concurrent;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Extensions;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class NodeRegistry(HashSet<Node> nodes, Node self)
{
    private readonly ConcurrentDictionary<string, HashSet<Node>> visitedNodesBySessionIds = new();

    public HashSet<Node> Nodes { get; } = nodes;
    public Node Self { get; } = self;

    public void AddNode(Node newNode)
    {
        this.Nodes.Add(newNode);
        Console.WriteLine(
            $"[{nameof(NodeRegistry)}] Added a new node [{newNode}] to the registry. " +
            $"The current registry content: [{this.Nodes.ToSeparatedString()}]");
    }

    public void RemoveNode(Node node)
    {
        this.Nodes.Remove(node);
        Console.WriteLine(
            $"[{nameof(NodeRegistry)}] Removed the node [{node}] from the registry. " +
            $"The current registry content: [{this.Nodes.ToSeparatedString()}]");
    }

    public string? SendMessageToNode(Node target, string message)
    {
        try
        {
            using var clientSocket = new TcpClient(target.Host, target.Port);
            using var writer = new StreamWriter(clientSocket.GetStream());
            writer.AutoFlush = true;
            using var reader = new StreamReader(clientSocket.GetStream());
            Console.WriteLine($"[{nameof(NodeRegistry)}] Sending the message [{message}] to the node [{target}]");
            writer.WriteLine(message);
            return reader.ReadLine();
        }
        catch (IOException e)
        {
            Console.WriteLine(
                $"[{nameof(NodeRegistry)}] An error occured while trying to send the message [{message}] " +
                $"to the node [{target}]: {e.Message}");
            return Responses.Error;
        }
    }

    public string? SendMessageToNodesUntilFirstReply(string message, string sessionId)
    {
        var unvisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
        return this.SendRecursivelyUntilFirstReply(message, sessionId, unvisitedNodes);
    }

    private string? SendRecursivelyUntilFirstReply(string msgToSend, string sessionId, HashSet<Node> unvisitedNodes)
    {
        Console.WriteLine(
            $"[{nameof(NodeRegistry)}] Remaining unvisited nodes: [{unvisitedNodes.ToSeparatedString()}]");

        var visitedNode = unvisitedNodes.FirstOrDefault();
        if (visitedNode != null)
        {
            this.AddVisitedNode(visitedNode, sessionId);
            var response = this.SendMessageToNode(visitedNode, msgToSend);
            if (response != Responses.Error)
            {
                Console.WriteLine($"[{nameof(NodeRegistry)}] Successful response [{response}] from the node [{visitedNode}]");
                this.ClearVisitedNodes(sessionId);
                return response;
            }

            var newNotVisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
            return this.SendRecursivelyUntilFirstReply(msgToSend, sessionId, newNotVisitedNodes);
        }

        Console.WriteLine($"[{nameof(NodeRegistry)}] There are no unvisited nodes remaining. Returning ERROR.");

        return Responses.Error;
    }

    public Dictionary<Node, string> SendMessageToNodesAndGatherResponses(string message, string sessionId)
    {
        var unvisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
        return this.SendToAllNodesRecursive(message, sessionId, unvisitedNodes, new Dictionary<Node, string>());
    }

    private Dictionary<Node, string> SendToAllNodesRecursive(
        string message, string sessionId, HashSet<Node> unvisitedNodes, Dictionary<Node, string> accumulator)
    {
        Console.WriteLine(
            $"[{nameof(NodeRegistry)}] Remaining unvisited nodes: [{unvisitedNodes.ToSeparatedString()}]");

        var maybeNotVisitedNode = unvisitedNodes.FirstOrDefault();
        if (maybeNotVisitedNode != null)
        {
            this.AddVisitedNode(maybeNotVisitedNode, sessionId);
            var response = this.SendMessageToNode(maybeNotVisitedNode, message);
            Console.WriteLine(
                $"[{nameof(NodeRegistry)}] Response [{response}] from the node [{maybeNotVisitedNode}]");

            accumulator[maybeNotVisitedNode] = response;

            var newUnvisitedNodes = this.GetUnvisitedNodesForSession(sessionId);
            return this.SendToAllNodesRecursive(message, sessionId, newUnvisitedNodes, accumulator);
        }

        Console.WriteLine(
            $"[{nameof(NodeRegistry)}] There are no unvisited nodes remaining. " +
            $"Returned responses: [{accumulator.ToSeparatedString()}]");

        return accumulator;
    }

    public void AddVisitedNode(Node visitedNode, string sessionId)
    {
        Console.WriteLine($"[NodeRegistry] Trying to add the visited node [{visitedNode}]");

        this.visitedNodesBySessionIds.AddOrUpdate(sessionId, [visitedNode], (key, visitedNodes) =>
        {
            visitedNodes.Add(visitedNode);

            Console.WriteLine(
                $"[{nameof(NodeRegistry)}] Added the visited node [{visitedNode}] for the session [{sessionId}]. " +
                $"The current visited nodes: [{visitedNodes.ToSeparatedString()}]");

            return visitedNodes;
        });
    }

    private void ClearVisitedNodes(string sessionId)
    {
        Console.WriteLine($"[NodeRegistry] Trying to clear the visited nodes for the session [{sessionId}]");
        this.visitedNodesBySessionIds.TryRemove(sessionId, out _);
    }

    private HashSet<Node> GetUnvisitedNodesForSession(string sessionId)
    {
        var visitedNodes = this.visitedNodesBySessionIds.GetValueOrDefault(sessionId, []);
        return this.Nodes.Where(node => !visitedNodes.Contains(node)).ToHashSet();
    }

    public bool ProcessedSessionId(string sessionId)
    {
        return this.visitedNodesBySessionIds.ContainsKey(sessionId);
    }
}
