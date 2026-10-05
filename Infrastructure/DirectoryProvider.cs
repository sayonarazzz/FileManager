using FileManager.Core.Contracts;
using FileManager.Core;
using FileManager.Core.Models;

namespace FileManager.Infrastructure;

public class DirectoryProvider : IDirectoryProvider
{
    public OutcomeResult<ISystemEntry> GetEntries(string fullPath)
    {
        List<ISystemEntry> entries = new();
        List<ISystemEntry> particalEntries = new();

        foreach (string path in Directory.EnumerateFileSystemEntries(fullPath))
        {
            try
            {
                string name = Path.GetFileName(path);
                FileAttributes attributes = File.GetAttributes(path);

                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    entries.Add(new DirectoryItem(path, name));
                }
                else
                {
                    long size = new FileInfo(path).Length;
                    entries.Add(new FileItem(path, name, size));
                }
            }
            catch (Exception)
            {
                string name = Path.GetFileName(path);
                particalEntries.Add(new DirectoryItem(path, name));
            }
        }

        IReadOnlyList<ISystemEntry> sortedEntries = entries
            .OrderByDescending(e => e is DirectoryItem)
            .ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase) // второе правило - не учитывать регистр
            .ToList();
        IReadOnlyList<ISystemEntry> sortedParticalEntries = particalEntries
            .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        return new OutcomeResult<ISystemEntry>(sortedEntries, sortedParticalEntries);
    }
}