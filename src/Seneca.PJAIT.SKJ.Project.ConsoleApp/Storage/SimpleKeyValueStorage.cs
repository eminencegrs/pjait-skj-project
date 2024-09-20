namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

public class SimpleKeyValueStorage
{
    private int key;
    private int value;

    public void NewPair(Pair pair)
    {
        lock (this)
        {
            this.key = pair.Key;
            this.value = pair.Value;
        }
    }

    public bool KeyExists(int keyToCheck)
    {
        return this.key == keyToCheck;
    }

    public Pair? GetValue(int keyToFind)
    {
        return this.KeyExists(keyToFind) ? this.GetPair() : null;
    }

    public Pair? SetNewValue(int existingKey, int newValue)
    {
        lock (this)
        {
            if (this.KeyExists(existingKey))
            {
                this.value = newValue;
                return this.GetPair();
            }
            else
            {
                return null;
            }
        }
    }

    public Pair GetPair()
    {
        return new Pair(this.key, this.value);
    }
}
