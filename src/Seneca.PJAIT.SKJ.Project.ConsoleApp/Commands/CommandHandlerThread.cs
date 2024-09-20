using System.Net;
using System.Net.Sockets;
using System.Text;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands.Handler;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands.Models;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class CommandHandlerThread : IDisposable
{
    private readonly TcpClient client;
    private readonly TcpListener server;
    private readonly Dictionary<string, CommandHandler> commandHandlers;
    private bool isDisposed = false;

    public CommandHandlerThread(TcpListener server, TcpClient client, Dictionary<string, CommandHandler> commandHandlers)
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
            var hostAddress = ((IPEndPoint)this.client.Client.RemoteEndPoint).Address.ToString();
            var receivedCommand = Command.Parse(receivedString);
            Console.WriteLine($"[CommandHandlerThread] Command received: [{receivedCommand}]. From: [{hostAddress}:{this.client.Client.RemoteEndPoint}]");

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

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.client.Close();
            this.client.Dispose();
            this.server.Stop();
            this.server.Dispose();
        }

        this.isDisposed = true;
    }

    ~CommandHandlerThread()
    {
        this.Dispose(false);
    }
}
