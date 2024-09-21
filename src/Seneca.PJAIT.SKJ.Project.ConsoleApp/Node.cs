namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public record Node
{
    public Node(string host, int port)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(port);

        this.Host = host;
        this.Port = port;
    }

    public string Host { get; }

    public int Port { get; }

    public override string ToString() => $"{this.Host}:{this.Port}";

    public override int GetHashCode() => HashCode.Combine(this.Host, this.Port);

    public static Node Parse(string inputString)
    {
        ArgumentException.ThrowIfNullOrEmpty(inputString);
        var parts = inputString.Split(':');
        return new Node(parts[0], int.Parse(parts[1]));
    }
}
