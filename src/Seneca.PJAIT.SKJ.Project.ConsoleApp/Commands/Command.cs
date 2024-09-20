namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Commands;

public class Command(string operation, string? argument = null)
{
    public string Operation { get; } = operation;
    public string? Argument { get; } = argument;

    public static Command Parse(string input)
    {
        string[] splitInput = input.Split(' ');
        string operation = splitInput[0];

        if (splitInput.Length < 2)
        {
            return new Command(operation);
        }

        string argument = splitInput[1];
        return new Command(operation, argument);
    }

    public string Serialize() =>
        string.IsNullOrEmpty(this.Argument) ? this.Operation : $"{this.Operation} {this.Argument}";

    public override string ToString() =>
        $"Command{{operation='{this.Operation}', argument={this.Argument}}}";
}
