namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public record KeyValueRecord(string Key, string Value)
{
    public override string ToString() => $"{this.Key}:{this.Value}";

    public static KeyValueRecord Parse(string inputString)
    {
        var parts = inputString.Split(':');
        return new KeyValueRecord(parts[0], parts[1]);
    }
}
