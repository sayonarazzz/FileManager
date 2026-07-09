using ConsoleFileManager.Enum;
namespace ConsoleFileManager.Infrastructure;

public class DirectoryProvider : IDirectoryProvider 
{
    public bool Exists(string path) // существование папки по этому пути
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public string? GetParentDirectory(string path)
    {
        string? parentPath = Directory.GetParent(path)?.FullName;
        
        return parentPath;
    }

    public string[] GetDirectories(string path)
    {
        string[] paths = Directory.GetDirectories(path);
        return paths.Select(directory => Path.GetFileName(directory)).ToArray();
    }

    public string[] GetFiles(string path)
    {
        string[] paths = Directory.GetFiles(path);
        return paths.Select(file => Path.GetFileName(file)).ToArray();
    }

    public ItemType GetItemType(string path)
    {
        if (Directory.Exists(path))
        {
            return ItemType.Directory;
        }

        if (File.Exists(path))
        {
            return ItemType.File;
        }
        
        return ItemType.None;
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    public void DeleteDirectory(string path)
    {
        Directory.Delete(path, true);
    }

    public void RenameFile(string oldPath, string newPath)
    {
        File.Move(oldPath, newPath);
    }
    
    public void RenameDirectory(string oldPath, string newPath)
    {
        Directory.Move(oldPath, newPath);
    }

    public void MoveFile(string oldPath, string newPath)
    {
        File.Move(oldPath, newPath);
    }

    public void MoveDirectory(string oldPath, string newPath)
    {
        Directory.Move(oldPath, newPath);
    }

    public void CopyFile(string oldPath, string newPath)
    {
        File.Copy(oldPath, newPath);
    }

    public void CopyDirectory(string oldPath, string newPath)
    {
        throw new NotSupportedException();
    }
}