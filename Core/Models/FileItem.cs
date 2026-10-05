using FileManager.Core.Contracts;
using FileManager.Core.Policies;

namespace FileManager.Core.Models;

public sealed class FileItem : ISystemEntry
{
    public string Name { get; }
    public string FullPath { get; }
    public long Size { get; }
    
    public bool IsTextFile => TextFilePolicy.IsTextFile(Name);
    
    public bool TextReadLimit() => Size > TextFilePolicy.MaxReadBytes;

    public FileItem(string fullPath, string name, long size)
    {
        FullPath = fullPath;
        Name = name;
        Size = size;
    }
}