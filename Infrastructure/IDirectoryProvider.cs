namespace ConsoleFileManager.Infrastructure;

public interface IDirectoryProvider
{
    void Delete(string path);
    bool Exists(string path);
    string? GetParentDirectory(string path);
    void Rename(string oldPath, string newPath);
    string[]  GetDirectories(string path);
    string[] GetFiles(string path);
    void Create(string path);
}