using ConsoleFileManager.Enum;
namespace ConsoleFileManager.Infrastructure;

public static class FileSystemInspector
{
    public static ItemType GetItemType(string path)
    {
        if (IsFile(path))
        {
            return ItemType.File;
        }

        if (IsDirectory(path))
        {
            return ItemType.Directory;
        }
        
        return ItemType.None;
    }

    private static bool IsFile(string path)
    {
        return File.Exists(path);
    }

    private static bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }
    
}