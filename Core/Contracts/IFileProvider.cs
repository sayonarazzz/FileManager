namespace FileManager.Core.Contracts;

public interface IFileProvider
{
    bool Exists(string fullPath);
    long GetLength(string fullPath);
    string ReadAllText(string fullPath);
    void AppendAllText(string fullPath, string text);
    void CreateEmpty(string fullPath);
    void Delete(string fullPath);
}