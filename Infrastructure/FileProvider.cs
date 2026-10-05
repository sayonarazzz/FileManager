using FileManager.Core.Contracts;

namespace FileManager.Infrastructure;

public class FileProvider : IFileProvider
{
    public bool Exists(string fullPath) => File.Exists(fullPath);

    public long GetLength(string fullPath) => new FileInfo(fullPath).Length;

    public string ReadAllText(string fullPath) => File.ReadAllText(fullPath);

    public void AppendAllText(string fullPath, string text) =>
        File.AppendAllText(fullPath, text);

    public void CreateEmpty(string fullPath)
    {
        using FileStream stream = File.Create(fullPath);
    }

    public void Delete(string fullPath) => File.Delete(fullPath);
}