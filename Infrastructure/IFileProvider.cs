namespace ConsoleFileManager.Infrastructure;

public interface IFileProvider
{
    Task<string> ReadAsync(string path);
    Task WriteAsync(string path, string text);
    Task AppendAsync(string path, string text);
}