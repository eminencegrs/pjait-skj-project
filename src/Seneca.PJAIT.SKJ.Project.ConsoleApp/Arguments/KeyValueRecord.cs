namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Arguments;

public class KeyValueRecord(string key, string value)
{
    public string Key { get; } = key;
    public string Value { get; } = value;

    public static KeyValueRecord Parse(string inputString)
    {
        string[] parts = inputString.Split(':');
        return new KeyValueRecord(parts[0], parts[1]);
    }

    public override string ToString() => $"{this.Key}:{this.Value}";
}
