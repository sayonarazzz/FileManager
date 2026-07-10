namespace ConsoleFileManager.Infrastructure;

public class DirectoryProvider : IDirectoryProvider 
{
    public void Delete(string path)
    {
        Directory.Delete(path, true);
    }
    
    public bool Exists(string path)
    {
        return Directory.Exists(path);
    }

    public string? GetParentDirectory(string path)
    {
        return Directory.GetParent(path)?.FullName;
    }

    public void Rename(string oldPath, string newPath)
    {
        Directory.Move(oldPath, newPath);
    }
    
    public string[] GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }
    
    public void Create(string path)
    {
        Directory.CreateDirectory(path);
    }
}