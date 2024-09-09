using System.Net;
using System.Net.Sockets;
using Seneca.PJAIT.SKJ.Project.Arguments;
using Seneca.PJAIT.SKJ.Project.Commands;
using Seneca.PJAIT.SKJ.Project.Commands.Handler;
using Seneca.PJAIT.SKJ.Project.Commands.Handler.Client;
using Seneca.PJAIT.SKJ.Project.Commands.Handler.Internal;
using Seneca.PJAIT.SKJ.Project.Commands.Models;
using Seneca.PJAIT.SKJ.Project.Communication;
using Seneca.PJAIT.SKJ.Project.Storage;

namespace Seneca.Pjait.Skj.Project;

public class DatabaseNode
{
    public async Task Run(string[] args)
    {
        Console.WriteLine("[DatabaseNode] Starting...");

        // Parse arguments.
        ParsedArgs parsedArgs = ArgumentParser.ParseArgs(args);

        //Console.WriteLine($"[DatabaseNode] Parsed args = {parsedArgs}");

        // Init a key value storage.
        int initialKey = int.Parse(parsedArgs.Record.Key);
        int initialValue = int.Parse(parsedArgs.Record.Value);
        SimpleKeyValueStorage kvStorage = new SimpleKeyValueStorage();
        kvStorage.NewPair(new Pair(initialKey, initialValue));

        // Init a node registry.
        Node? self = null;
        try
        {
            self = new Node(Dns.GetHostAddresses(Dns.GetHostName())[0].ToString(), parsedArgs.TcpPort);
        }
        catch (Exception ex)
        {
            throw new Exception("Unknown host", ex);
        }

        NodeRegistry nodeRegistry = new NodeRegistry(new HashSet<Node>(), self);

        try
        {
            foreach (var newNode in parsedArgs.Connect)
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

        // Init command handlers.
        var commandHandlerMap = new Dictionary<string, CommandHandler>();
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

        // Open the socket and start listening.
        try
        {
            using (var serverSocket = new TcpListener(IPAddress.Any, parsedArgs.TcpPort))
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
