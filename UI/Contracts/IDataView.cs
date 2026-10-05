using FileManager.Enum;
using FileManager.Core.Contracts;
using FileManager.Core.Models;
using FileManager.Core;

namespace FileManager.UI.Contracts;

public interface IDataView
{
    void ShowDrives(IReadOnlyList<DriveItem> drives);
    void ShowPartialDrives(int count, IReadOnlyList<DriveItem> partialDrives);
    void ShowSelectedDrive(string drivePath);
    void ShowEntries(IReadOnlyList<ISystemEntry> entries);
    void ShowPartialEntries(int count, IReadOnlyList<ISystemEntry> partialEntries);
    void ShowTextFileContent(string fileName, string data);
    void ShowError(FileError error);
}