namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

public interface IKeyValueStorage
{
    void SetKeyValue(Pair other);
    //Pair? SetKeyValue(int existingKey, int newValue);
    Pair? GetValue(int keyToFind);
    Pair? SetNewValue(int existingKey, int newValue);
    Pair GetPair();
}

public class KeyValueStorage : IKeyValueStorage
{
    private readonly object lockObject = new();

    private int key;
    private int value;

    public void SetKeyValue(Pair other)
    {
        lock (this.lockObject)
        {
            this.key = other.Key;
            this.value = other.Value;
        }
    }

    public Pair? GetValue(int keyToFind)
    {
        return this.KeyExists(keyToFind) ? this.GetPair() : null;
    }

    public Pair? SetNewValue(int existingKey, int newValue)
    {
        lock (this.lockObject)
        {
            if (this.KeyExists(existingKey))
            {
                this.value = newValue;
                return this.GetPair();
            }

            return null;
        }
    }

    public Pair GetPair() => new(this.key, this.value);

    private bool KeyExists(int keyToCheck) => this.key == keyToCheck;
}
