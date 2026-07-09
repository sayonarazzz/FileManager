namespace ConsoleFileManager.Core;

public class TextDocument
{
    public string FullPath { get; }
    
    public string Text { get; set; }

    public bool IsModified { get; set; } = false;

    public TextDocument(string fullPath, string text)
    {
        FullPath = fullPath;
        Text = text;
    }
}