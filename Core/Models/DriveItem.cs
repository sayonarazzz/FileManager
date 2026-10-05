using FileManager.Enum;
namespace FileManager.Core.Models;

public sealed class DriveItem 
{
    public char Letter { get; }
    public string Name { get; }
    public string Label { get; }
    public DriveTypes DriveType { get; }
    public bool IsReady { get; }

    public DriveItem(char letter, string name, string label, DriveTypes driveType, bool isReady)
    {
        Letter = letter;
        Name = name;
        Label = label;
        DriveType = driveType;
        IsReady = isReady;
    }
}