namespace Seneca.PJAIT.SKJ.Project.ConsoleApp;

public record KeyValueRecord(string Key, string Value)
{
    public static KeyValueRecord Parse(string inputString)
    {
        string[] parts = inputString.Split(':');
        return new KeyValueRecord(parts[0], parts[1]);
    }

    public override string ToString() => $"{this.Key}:{this.Value}";
}
