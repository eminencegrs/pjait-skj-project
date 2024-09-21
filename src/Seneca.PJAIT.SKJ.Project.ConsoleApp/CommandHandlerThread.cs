using System.Net.Sockets;
using System.Text;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public class CommandHandlerThread(
    TcpListener server,
    TcpClient client,
    ICommandHandlerProvider factory)
{
    public void Run()
    {
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            var receivedString = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
            var receivedCommand = Command.Parse(receivedString);

            Console.WriteLine(
                $"[{nameof(CommandHandlerThread)}] Command received: [{receivedCommand}]. " +
                $"From: [{client.Client.RemoteEndPoint}]");

            if (factory.TryGetHandler(receivedCommand.Operation, out var handler))
            {
                var response = handler.Handle(receivedCommand, Guid.NewGuid().ToString());
                var responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                Console.WriteLine(
                    $"[{nameof(CommandHandlerThread)}] The [{receivedCommand.Operation}] operation " +
                    "is not supported");

                var errorResponseBytes = Encoding.ASCII.GetBytes(Responses.Error);
                stream.Write(errorResponseBytes, 0, errorResponseBytes.Length);
            }

            if (receivedCommand.Operation == "terminate")
            {
                Console.WriteLine(
                    $"[{nameof(CommandHandlerThread)}] Received the terminate command. Terminating...");

                client.Close();
                server.Stop();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"{nameof(CommandHandlerThread)} failed because of an internal exception.", ex);
        }
    }
}
