using FileManager.Core.Models;

namespace FileManager.Core.Contracts;

public interface IDriveProvider
{
    OutcomeResult<DriveItem> GetDrives();
}

