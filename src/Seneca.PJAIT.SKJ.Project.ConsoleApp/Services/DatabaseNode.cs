using System.Net;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Extensions;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;

public class DatabaseNode : IDatabaseNode
{
    private readonly IKeyValueStorage storage;
    public DatabaseNode(IKeyValueStorage storage)
    {
        this.storage = storage;
    }

    public async Task Run(int tcpPort, KeyValueRecord record, IReadOnlyCollection<Node> nodes)
    {
        Console.WriteLine($"[{nameof(DatabaseNode)}] Starting...");
        
        var nodesString = nodes.Count == 0 ? "<none>" : string.Join(", ", nodes);
        
        Console.WriteLine(
            $"[{nameof(DatabaseNode)}] Options: " +
            $"tcpPort='{tcpPort}', record='{record}', nodes='{nodesString}'.");
        
        this.storage.SetKeyValue(record.ToPair());
        
        Node? self = default;
        try
        {
            var host = (await Dns.GetHostAddressesAsync(Dns.GetHostName()))[0].ToString();
            self = new Node(host, tcpPort);
        }
        catch (Exception ex)
        {
            throw new Exception("Unknown host", ex);
        }
        
        var nodeRegistry = new NodeRegistry([], self);
        foreach (var newNode in nodes)
        {
            var addNodeCommand = new AddNodeCommand(nodeRegistry.Self);
            var response = nodeRegistry.SendMessageToNode(newNode, addNodeCommand.Serialize());
        
            // TODO: Improve it.
            nodeRegistry.AddNode(Node.Parse(response));
        }
        
        var commandHandlerMap = new Dictionary<string, CommandHandlerBase>();
        commandHandlerMap.Add(SetValueCommandHandler.OperationName, new SetValueCommandHandler(this.storage, nodeRegistry));
        commandHandlerMap.Add(GetValueCommandHandler.OperationName, new GetValueCommandHandler(this.storage, nodeRegistry));
        commandHandlerMap.Add(FindKeyCommandHandler.OperationName, new FindKeyCommandHandler(this.storage, nodeRegistry));
        commandHandlerMap.Add(GetMaxCommandHandler.OperationName, new GetMaxCommandHandler(this.storage, nodeRegistry));
        commandHandlerMap.Add(GetMinCommandHandler.OperationName, new GetMinCommandHandler(this.storage, nodeRegistry));
        commandHandlerMap.Add(NewRecordCommandHandler.OperationName, new NewRecordCommandHandler(this.storage));
        commandHandlerMap.Add(AddNodeCommandHandler.OperationName, new AddNodeCommandHandler(nodeRegistry));
        commandHandlerMap.Add(RemoveNodeCommandHandler.OperationName, new RemoveNodeCommandHandler(nodeRegistry));
        commandHandlerMap.Add(TerminateCommandHandler.OperationName, new TerminateCommandHandler(nodeRegistry));
        commandHandlerMap.Add(ForwardCommandHandler.OperationName, new ForwardCommandHandler(commandHandlerMap, nodeRegistry));
        
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
                _ = Task.Run(() => new CommandHandlerThread(serverSocket, clientSocket, commandHandlerMap).Run());
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
}
