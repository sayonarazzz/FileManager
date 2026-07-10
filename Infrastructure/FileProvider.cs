namespace ConsoleFileManager.Infrastructure;

public class FileProvider : IFileProvider
{
    public async Task<string> ReadAsync(string path)
    {
        string text = await File.ReadAllTextAsync(path);
        return text;
    }

    public async Task WriteAsync(string path, string text)
    {
        await File.WriteAllTextAsync(path, text);
    }

    public async Task AppendAsync(string path, string text)
    {
        await File.AppendAllTextAsync(path, text);
    }

    public void Delete(string path)
    {
        File.Delete(path);
    }

    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public void Rename(string oldPath, string newPath)
    {
        File.Move(oldPath, newPath);
    }
}