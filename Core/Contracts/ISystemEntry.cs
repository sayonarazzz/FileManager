namespace FileManager.Core.Contracts;

public interface ISystemEntry
{
    string Name { get; }
    string FullPath { get; }
}