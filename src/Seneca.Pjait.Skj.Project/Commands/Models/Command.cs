namespace Seneca.PJAIT.SKJ.Project.Commands.Models;

public class Command
{
    public Command(string operation, string? argument = null)
    {
        this.Operation = operation;
        this.Argument = argument;
    }

    public string Operation { get; }
    public string? Argument { get; }

    public static Command Parse(string input)
    {
        string[] splitInput = input.Split(' ');
        string operation = splitInput[0];

        if (splitInput.Length >= 2)
        {
            string argument = splitInput[1];
            return new Command(operation, argument);
        }
        else
        {
            return new Command(operation);
        }
    }

    public string Serialize()
    {
        if (!string.IsNullOrEmpty(this.Argument))
        {
            return $"{this.Operation} {this.Argument}";
        }
        else
        {
            return this.Operation;
        }
    }

    public override string ToString()
    {
        return $"Command{{operation='{this.Operation}', argument={this.Argument}}}";
    }
}
