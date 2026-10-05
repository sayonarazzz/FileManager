using FileManager.Core.Models;
using FileManager.Core.Contracts;
using FileManager.Core;
using FileManager.Enum;

namespace FileManager.Infrastructure;

public class DriveProvider : IDriveProvider
{
    public OutcomeResult<DriveItem> GetDrives()
    {
        List<DriveItem> findDrives = new();                         // полностью прочитанные диски
        List<DriveItem> particalDrives = new();                     // не прочитанные либо полностью либо частично (но ОС все равно скорее всего вернет хотя бы букву диска)
        DriveInfo[] totalDrives = DriveInfo.GetDrives();            // получение дисков от ОС

        foreach (DriveInfo drive in totalDrives)
        {
            try
            {
                char letter = char.ToUpperInvariant(drive.Name[0]);
                findDrives.Add(new DriveItem(letter, drive.Name, drive.VolumeLabel, MapDriveType(drive.DriveType), drive.IsReady));
            }
            catch (Exception)
            {
                char letter = char.ToUpperInvariant(drive.Name[0]);
                particalDrives.Add(new DriveItem(letter, drive.Name, label: "", DriveTypes.Unknown, isReady: false));
            }
        }

        return new OutcomeResult<DriveItem>(findDrives, particalDrives);
    }

    private DriveTypes MapDriveType(DriveType type)
    {
        return type switch
        {
            DriveType.Fixed => DriveTypes.Fixed,
            DriveType.Removable => DriveTypes.Removable,
            DriveType.Network => DriveTypes.Network,
            DriveType.CDRom => DriveTypes.CDRom,
            DriveType.Ram => DriveTypes.Ram,
            _ => DriveTypes.Unknown
        };
    }
}