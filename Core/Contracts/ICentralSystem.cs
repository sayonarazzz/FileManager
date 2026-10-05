using FileManager.Core.Models;

namespace FileManager.Core.Contracts;

public interface ICentralSystem
{
    string? CurrentPath { get; }
    OperationResult<OutcomeResult<DriveItem>> GetDrives();
    OperationResult SelectDriveByLetter(char letter);
    OperationResult<OutcomeResult<ISystemEntry>> GetCurrentEntries();
    void ClearCurrentPath();
    OperationResult EnterDirectory(string entryName);
    OperationResult GoToParentDirectory();
    OperationResult<string> ReadTextFile(string entryName);
    OperationResult AppendTextFile(string entryName, string text);
    OperationResult CreateEmptyTextFile(string entryName);
    OperationResult DeleteTextFile(string entryName);
}