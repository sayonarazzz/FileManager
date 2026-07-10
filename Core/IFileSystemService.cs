namespace ConsoleFileManager.Core;

public interface IFileSystemService
{
    Task<OperationResult<string>> TryOpenFileAsync(string path);
    Task<bool> TryCreateFileAsync(string path, string text);
    Task<bool> TryAppendFileAsync(string path, string text);
    bool TryDelete(string name);
    bool TryBackDirectory();
    bool TryGoToDirectory(string name);
    bool TryGoToAbsolutePath(string path);
    bool TryRename(string oldName, string newName);
    bool TryCreateDirectory(string name);
    string GetCurrentPath();
    string[]  GetDirectories();
    string[] GetFiles();
}