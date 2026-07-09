using ConsoleFileManager.Infrastructure;

namespace ConsoleFileManager.Core;

public class FileService<T> : IFileService<T>
{
    private readonly IFileProvider fileProvider;
    private readonly ISerializer<T> serializer;
    
    public FileService(IFileProvider provider, ISerializer<T> serializer)
    {
        fileProvider = provider;
        this.serializer = serializer;
    }
    
    public async Task<T> ReadAsync(string path)
    {
        string text = await fileProvider.ReadAsync(path);
        return serializer.Deserialize(text);
    }

    public async Task WriteAsync(string path, T text)
    {
        string data = serializer.Serialize(text);
        await fileProvider.WriteAsync(path, data);
    }

    public async Task AppendAsync(string path, T text)
    {
        string data = serializer.Serialize(text);
        await fileProvider.AppendAsync(path, data);
    }
}