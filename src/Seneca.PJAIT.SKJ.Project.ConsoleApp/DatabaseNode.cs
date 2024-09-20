using System.Net;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Communication;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class DatabaseNode
{
    public async Task Run(int tcpPort, KeyValueRecord record, IReadOnlyCollection<Node> nodes)
    {
        Console.WriteLine($"[{nameof(DatabaseNode)}] Starting...");

        var nodesString = nodes.Any() ? string.Join(", ", nodes) : "<none>";
        Console.WriteLine(
            $"[{nameof(DatabaseNode)}] Options: " +
            $"tcpPort='{tcpPort}', record='{record}', nodes='{nodesString}'.");

        int initialKey = int.Parse(record.Key);
        int initialValue = int.Parse(record.Value);
        SimpleKeyValueStorage kvStorage = new SimpleKeyValueStorage();
        kvStorage.NewPair(new Pair(initialKey, initialValue));

        Node? self = null;
        try
        {
            self = new Node(Dns.GetHostAddresses(Dns.GetHostName())[0].ToString(), tcpPort);
        }
        catch (Exception ex)
        {
            throw new Exception("Unknown host", ex);
        }

        NodeRegistry nodeRegistry = new NodeRegistry(new HashSet<Node>(), self);

        try
        {
            foreach (var newNode in nodes)
            {
                try
                {
                    var addNodeCmd = new AddNodeCommand(nodeRegistry.Self);
                    string hostPortStr = nodeRegistry.SendMessageToNode(newNode, addNodeCmd.Serialize());
                    nodeRegistry.AddNode(Node.Parse(hostPortStr));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var commandHandlerMap = new Dictionary<string, CommandHandlerBase>();
        commandHandlerMap.Add(SetValueCommandHandler.OperationName, new SetValueCommandHandler(kvStorage, nodeRegistry));
        commandHandlerMap.Add(GetValueCommandHandler.OperationName, new GetValueCommandHandler(kvStorage, nodeRegistry));
        commandHandlerMap.Add(FindKeyCommandHandler.OperationName, new FindKeyCommandHandler(kvStorage, nodeRegistry));
        commandHandlerMap.Add(GetMaxCommandHandler.OperationName, new GetMaxCommandHandler(kvStorage, nodeRegistry));
        commandHandlerMap.Add(GetMinCommandHandler.OperationName, new GetMinCommandHandler(kvStorage, nodeRegistry));
        commandHandlerMap.Add(NewRecordCommandHandler.OperationName, new NewRecordCommandHandler(kvStorage));
        commandHandlerMap.Add(AddNodeCommandHandler.OperationName, new AddNodeCommandHandler(nodeRegistry));
        commandHandlerMap.Add(RemoveNodeCommandHandler.OperationName, new RemoveNodeCommandHandler(nodeRegistry));
        commandHandlerMap.Add(TerminateCommandHandler.OperationName, new TerminateCommandHandler(nodeRegistry));
        commandHandlerMap.Add(ForwardCommandHandler.OperationName, new ForwardCommandHandler(commandHandlerMap, nodeRegistry));

        try
        {
            using (var serverSocket = new TcpListener(IPAddress.Any, tcpPort))
            {
                serverSocket.Start();
                Console.WriteLine($"[DatabaseNode] Start listening for incoming connections on [{serverSocket.LocalEndpoint}]...");

                while (true)
                {
                    var clientSocket = await serverSocket.AcceptTcpClientAsync();
                    _ = Task.Run(() => new CommandHandlerThread(serverSocket, clientSocket, commandHandlerMap).Run());
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[DatabaseNode] Received socket exception: {ex.Message}");
        }
    }
}
