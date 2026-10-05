namespace FileManager.Core.Contracts;

public interface IDirectoryProvider
{
    OutcomeResult<ISystemEntry> GetEntries(string fullPath);
}