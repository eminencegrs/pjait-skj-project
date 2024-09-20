using System.Text.RegularExpressions;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class ForwardCommandArgument
{
    private static readonly string OperationRegex = "op=([^;\\r\\n]*)";
    private static readonly string SessionIdRegex = "sid=([^;\\r\\n]*)";
    private static readonly string ArgumentRegex = "arg=([^;\\r\\n]*)";
    private static readonly string FromRegex = "from=([^;\\r\\n]*)";

    public string ClientOperation { get; }
    public string SessionId { get; }
    public Node From { get; }
    public string? Argument { get; }

    public ForwardCommandArgument(
        string clientOperation,
        string sessionId,
        Node from,
        string? argument = null)
    {
        this.ClientOperation = clientOperation;
        this.SessionId = sessionId;
        this.From = from;
        this.Argument = argument;
    }

    public static ForwardCommandArgument Parse(string input)
    {
        var operationMatch = Regex.Match(input, OperationRegex);
        var sessionIdMatch = Regex.Match(input, SessionIdRegex);
        var argumentMatch = Regex.Match(input, ArgumentRegex);
        var fromMatch = Regex.Match(input, FromRegex);

        if (!operationMatch.Success)
        {
            var errMsg = $"InternalCommandArgument parsing error: operation not found in [{input}]";
            Console.WriteLine(errMsg);
            throw new Exception(errMsg);
        }

        if (!sessionIdMatch.Success)
        {
            var errMsg = $"InternalCommandArgument parsing error: sessionId not found in [{input}]";
            Console.WriteLine(errMsg);
            throw new Exception(errMsg);
        }

        if (!fromMatch.Success)
        {
            var errMsg = $"InternalCommandArgument parsing error: `from` address not found in [{input}]";
            Console.WriteLine(errMsg);
            throw new Exception(errMsg);
        }

        string? maybeArgument = null;
        if (argumentMatch.Success)
        {
            maybeArgument = argumentMatch.Groups[1].Value;
        }

        return new ForwardCommandArgument(
            operationMatch.Groups[1].Value,
            sessionIdMatch.Groups[1].Value,
            Node.Parse(fromMatch.Groups[1].Value),
            maybeArgument);
    }

    public string Serialize()
    {
        return this.Argument == null
            ? $"op={this.ClientOperation};sid={this.SessionId};from={this.From}"
            : $"op={this.ClientOperation};sid={this.SessionId};from={this.From};arg={this.Argument}";
    }
}
