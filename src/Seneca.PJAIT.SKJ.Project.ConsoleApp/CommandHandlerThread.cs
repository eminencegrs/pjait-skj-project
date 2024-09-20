using System.Net.Sockets;
using System.Text;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class CommandHandlerThread
{
    private readonly TcpClient client;
    private readonly TcpListener server;
    private readonly Dictionary<string, CommandHandlerBase> commandHandlers;

    public CommandHandlerThread(TcpListener server, TcpClient client, Dictionary<string, CommandHandlerBase> commandHandlers)
    {
        this.client = client;
        this.server = server;
        this.commandHandlers = commandHandlers;
    }

    public void Run()
    {
        try
        {
            using var stream = this.client.GetStream();
            var buffer = new byte[1024];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            var receivedString = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
            var receivedCommand = Command.Parse(receivedString);
            Console.WriteLine($"[CommandHandlerThread] Command received: [{receivedCommand}]. From: [{this.client.Client.RemoteEndPoint}]");

            if (this.commandHandlers.TryGetValue(receivedCommand.Operation, out var handler))
            {
                var response = handler.Handle(receivedCommand, Guid.NewGuid().ToString());
                var responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                Console.WriteLine($"[CommandHandlerThread] Operation [{receivedCommand.Operation}] not supported");
                var errorResponseBytes = Encoding.ASCII.GetBytes(Responses.Error);
                stream.Write(errorResponseBytes, 0, errorResponseBytes.Length);
            }

            if (receivedCommand.Operation == "terminate")
            {
                Console.WriteLine("[CommandHandlerThread] Received terminate signal. Terminating...");
                this.client.Close();
                this.server.Stop();
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
