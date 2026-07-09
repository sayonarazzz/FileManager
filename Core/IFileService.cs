namespace ConsoleFileManager.Core;

public interface IFileService<T>
{ 
    Task<T> ReadAsync(string path);
    Task WriteAsync(string path, T text);
    Task AppendAsync(string path, T text);
}