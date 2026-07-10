namespace ConsoleFileManager.Core;

public class Document
{
    public string FullPath { get; }
    public string Text { get; set; }
    public bool IsModified { get; set; }
    
    public Document(string fullPath, string text)
    {
        FullPath = fullPath;
        Text = text;
    }
}