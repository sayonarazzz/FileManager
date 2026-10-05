using FileManager.Core.Contracts;

namespace FileManager.Core.Models;

public sealed class DirectoryItem : ISystemEntry
{
    public string Name { get; }
    public string FullPath { get; }
    
    public DirectoryItem(string fullPath, string name)
    {
        FullPath = fullPath;
        Name = name;
    }
}