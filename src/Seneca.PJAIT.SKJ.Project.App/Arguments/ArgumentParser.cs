namespace Seneca.PJAIT.SKJ.Project.Arguments;

public class ArgumentParser
{
    public static ParsedArgs ParseArgs(string[] args)
    {
        ParsedArgs parsedArgs = new ParsedArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if ("-tcpport".Equals(args[i], StringComparison.OrdinalIgnoreCase))
            {
                parsedArgs.TcpPort = int.Parse(args[++i]);
            }
            else if ("-record".Equals(args[i], StringComparison.OrdinalIgnoreCase))
            {
                parsedArgs.Record = KeyValueRecord.Parse(args[++i]);
            }
            else if ("-connect".Equals(args[i], StringComparison.OrdinalIgnoreCase))
            {
                parsedArgs.AddConnectNode(Node.Parse(args[++i]));
            }
        }
        return parsedArgs;
    }
}
