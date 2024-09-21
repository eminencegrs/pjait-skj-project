using System.Net;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Extensions;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

internal class DatabaseNode(
    IKeyValueStorage storage,
    INodeRegistry registry,
    ICommandHandlerProvider factory)
    : IDatabaseNode
{
    public async Task Run(int tcpPort, KeyValueRecord record, IReadOnlyCollection<Node> nodes)
    {
        Console.WriteLine($"[{nameof(DatabaseNode)}] Starting...");

        var nodesString = nodes.Count == 0 ? "<none>" : string.Join(", ", nodes);

        Console.WriteLine(
            $"[{nameof(DatabaseNode)}] Options: " +
            $"tcpPort='{tcpPort}', record='{record}', nodes='{nodesString}'.");

        storage.SetKeyValue(record.ToPair());

        Node? thisNode = default;
        try
        {
            var host = (await Dns.GetHostAddressesAsync(Dns.GetHostName()))[0].ToString();
            thisNode = new Node(host, tcpPort);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unknown host", ex);
        }

        registry.Initialize(thisNode);

        this.AddNeighbors(nodes, thisNode);

        // TODO: Refactoring.
        try
        {
            using var serverSocket = new TcpListener(IPAddress.Any, tcpPort);
            serverSocket.Start();
            Console.WriteLine(
                $"[{nameof(DatabaseNode)}] Start listening for incoming connections on [{serverSocket.LocalEndpoint}]...");

            while (true)
            {
                var clientSocket = await serverSocket.AcceptTcpClientAsync();
                _ = Task.Run(() => new CommandHandlerThread(serverSocket, clientSocket, factory).Run());
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[{nameof(DatabaseNode)}] SocketException: {ex.Message}");
        }
        catch (ThreadAbortException ex)
        {
            Console.WriteLine($"[{nameof(DatabaseNode)}] ThreadAbortException: {ex.Message}");
        }
    }

    private void AddNeighbors(IReadOnlyCollection<Node> nodes, Node thisNode)
    {
        foreach (var connectedNodes in nodes)
        {
            var addNodeCommand = new AddNodeCommand(thisNode);
            var response = registry.SendMessageToNode(connectedNodes, addNodeCommand.Serialize());
            registry.AddConnectedNode(Node.Parse(response));
        }
    }
}
