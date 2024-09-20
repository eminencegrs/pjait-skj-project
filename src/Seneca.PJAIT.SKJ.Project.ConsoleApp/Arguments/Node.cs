namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Arguments;

public class Node(string host, int port)
{
    public string Host { get; } = host;
    public int Port { get; } = port;

    public static Node Parse(string inputString)
    {
        string[] parts = inputString.Split(':');
        return new Node(parts[0], int.Parse(parts[1]));
    }

    public override string ToString()
    {
        return $"{this.Host}:{this.Port}";
    }

    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        Node node = (Node)obj;

        return this.Port == node.Port && string.Equals(this.Host, node.Host, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Host, this.Port);
    }
}
