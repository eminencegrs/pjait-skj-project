namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public interface IDatabaseNode
{
    Task Run(int tcpPort, KeyValueRecord record, IReadOnlyCollection<Node> nodes);
}
