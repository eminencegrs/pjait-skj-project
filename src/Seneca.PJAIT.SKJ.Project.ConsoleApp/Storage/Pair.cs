namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

public record Pair(int Key, int Value)
{
    public static Pair Parse(string str)
    {
        var splitString = str.Split(':');
        return new Pair(int.Parse(splitString[0]), int.Parse(splitString[1]));
    }

    public override string ToString() => $"{this.Key}:{this.Value}";
}
