namespace FileManager.UI.Menu;

public sealed class MenuOption
{
    public int Key { get; }
    public string Message { get; }

    public MenuOption(int key, string message)
    {
        Key = key;
        Message = message;
    }
}