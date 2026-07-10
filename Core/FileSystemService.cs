using ConsoleFileManager.Enum;
using ConsoleFileManager.Infrastructure;
namespace ConsoleFileManager.Core;

public class FileSystemService : IFileSystemService
{
    private readonly IFileProvider fileProvider;
    private readonly IDirectoryProvider directoryProvider;
    private readonly string rootPath = @"C:\Proga";
    private string currentPath;

    public FileSystemService(IFileProvider fileProvider, IDirectoryProvider directoryProvider, string currentPath)
    {
        this.fileProvider = fileProvider;
        this.directoryProvider = directoryProvider;
        this.currentPath = currentPath;
    }
    
    private string GetFullPath(string name)
    {
        return Path.Combine(currentPath, name);
    }
    private bool IsValidPath(string path)
    {
        return path.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<OperationResult<string>> TryOpenFileAsync(string name) // открытие файла (получение данных)
    {
        string path = GetFullPath(name);

        if (!IsValidPath(path)) // выход за корневую
        {
            return new OperationResult<string>(false, null);
        }

        if (!fileProvider.Exists(path)) // существует ли файл с таким путем
        {
            return new OperationResult<string>(false, null);
        }

        if (path == currentPath) // введена актуальная папка
        {
            return new OperationResult<string>(false, null);
        }

        return new OperationResult<string>(true, await fileProvider.ReadAsync(path));
    }

    public async Task<bool> TryCreateFileAsync(string name, string text) // создание файла с данными
    {
        string path = GetFullPath(name);

        if (!IsValidPath(path))
        {
            return false;
        }
        
        if (path == currentPath) // введена актуальная папка
        {
            return false;
        }
        
        await fileProvider.WriteAsync(path, text);
        return true;
    }

    public async Task<bool> TryAppendFileAsync(string fullPath, string text)
    {
        if (!IsValidPath(fullPath))
        {
            return false;
        }

        if (!fileProvider.Exists(fullPath))
        {
            return false;
        }

        if (fullPath == currentPath)
        {
            return false;
        }

        await fileProvider.AppendAsync(fullPath, text);
        return true;
    }

    public bool TryDelete(string name)
    {
        string path =  GetFullPath(name);
        
        if (!IsValidPath(path))
        {
            return false;
        }
        
        if (path == currentPath)
        {
            return false;
        }

        ItemType type = FileSystemInspector.GetItemType(path);

        switch (type)
        {
            case ItemType.File:
                fileProvider.Delete(path);
                return true;
            
            case ItemType.Directory:
                directoryProvider.Delete(path);
                return true;
            
            default:
                return false;
        }
    }

    public bool TryBackDirectory() // возвращение назад
    {
        string? parentPath = directoryProvider.GetParentDirectory(currentPath);

        if (parentPath == null) // выход вообще в никуда
        {
            return false;
        }

        if (!IsValidPath(parentPath))
        {
            return false;
        }
        
        currentPath = parentPath;
        return true;
    }

    public bool TryGoToDirectory(string name) // передвижение вперед
    {
        string newPath = GetFullPath(name);
        
        if (!IsValidPath(newPath)) // выход за коренвую папку
        {
            return false;
        }
        
        if (!directoryProvider.Exists(newPath)) // существует ли папка с таким путем
        {
            return false;
        }

        if (newPath == currentPath) // введена актуальная папка (то есть пустой ввод)
        {
            return false;
        }
        
        currentPath =  newPath;
        return true;
    }

    public bool TryGoToAbsolutePath(string path) // передвижение по абсолютному пути
    {
        if (!IsValidPath(path)) // выход за коренвую папку
        {
            return false;
        }
        
        // позже обработаем случай ввода не папка в конце пути, а файла

        if (path == currentPath) // введена актуальная папка (то есть пустой ввод)
        {
            return false;
        }
        
        currentPath = path;
        return true;
    }

    public bool TryCreateDirectory(string name)
    {
        string path = GetFullPath(name);

        if (!IsValidPath(path))
        {
            return false;
        }

        if (directoryProvider.Exists(path))
        {
            return false;
        }

        directoryProvider.Create(path);
        return true;
    }

    public bool TryRename(string oldName, string newName)
    {
        string oldPath = GetFullPath(oldName);
        
        if (!IsValidPath(oldPath))
        {
            return false;
        }

        if (oldPath == currentPath) // введена актуальная папка
        {
            return false;
        }
        
        string newPath = GetFullPath(newName);
        
        if (!IsValidPath(newPath))
        {
            return false;
        }

        if (newPath == currentPath) // введена актуальная папка
        {
            return false;
        }
        
        ItemType type = FileSystemInspector.GetItemType(oldPath);

        switch (type)
        {
            case ItemType.File:
                fileProvider.Rename(oldPath, newPath);
                return true;
            
            case ItemType.Directory:
                directoryProvider.Rename(oldPath, newPath);
                return true;
            
            default:
                return false;
        }
    }
    
    public string GetCurrentPath()
    {
        return currentPath;
    }

    public string[] GetDirectories()
    {
        return directoryProvider.GetDirectories(currentPath);
    }

    public string[] GetFiles()
    {
        return directoryProvider.GetFiles(currentPath);
    }
}
    