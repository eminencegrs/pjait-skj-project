namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

public class Pair(int key, int value)
{
    public int Key { get; } = key;
    public int Value { get; } = value;

    public static Pair Parse(string str)
    {
        var splitString = str.Split(':');
        return new Pair(int.Parse(splitString[0]), int.Parse(splitString[1]));
    }

    public override string ToString() => $"{this.Key}:{this.Value}";
}
