namespace ConsoleFileManager.Core;

public class TextSerializer : ISerializer<string>
{
    public string Serialize(string text)
    {
        return text;
    }

    public string Deserialize(string text)
    {
        return text;
    }
}