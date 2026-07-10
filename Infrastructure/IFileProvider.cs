namespace ConsoleFileManager.Infrastructure;

public interface IFileProvider
{
    Task<string> ReadAsync(string path);
    Task WriteAsync(string path, string text);
    Task AppendAsync(string path, string text);
    void Delete(string path);
    bool Exists(string path);
    void Rename(string oldPath, string newPath);
}