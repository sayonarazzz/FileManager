using FileManager.Core.Contracts;
using FileManager.Core.Input;
using FileManager.Core.Models;
using FileManager.Core.Policies;
using FileManager.Enum;

namespace FileManager.Core;

public class CentralSystem : ICentralSystem
{
    private readonly IDriveProvider driveProvider;
    private readonly IDirectoryProvider directoryProvider;
    private readonly IFileProvider fileProvider;
    private IReadOnlyList<DriveItem>? cachedDrives;
    
    private string? currentRoot;
    public string? CurrentPath { get; private set; }

    public CentralSystem(IDriveProvider driveProvider, IDirectoryProvider directoryProvider,  IFileProvider fileProvider)
    {
        this.driveProvider = driveProvider;
        this.directoryProvider = directoryProvider;
        this.fileProvider = fileProvider;
    }

    public OperationResult<OutcomeResult<DriveItem>> GetDrives()
    {
        try
        {
            OutcomeResult<DriveItem> drives = driveProvider.GetDrives();

            if (drives.Items.Count == 0 && !drives.HasPartial)                             // ОС вернула пустой список дисков
            {
                return OperationResult<OutcomeResult<DriveItem>>.Failure(FileError.NoDrivesAvailable);  
            }

            cachedDrives = drives.Items.Concat(drives.PartialItems).ToList();
            return OperationResult<OutcomeResult<DriveItem>>.Success(drives);                     
        }
        catch (Exception)                                                                               
        {
            return OperationResult<OutcomeResult<DriveItem>>.Failure(FileError.DriveReadError);         // упало чтение дисков 
        }
    }

    public OperationResult SelectDriveByLetter(char driveLetter)
    {
        if (cachedDrives is null)
        {
            return OperationResult.Failure(FileError.DrivesNotLoaded);
        }

        if (cachedDrives.Count == 0)
        {
            return OperationResult.Failure(FileError.NoDrivesAvailable);
        }
        
        DriveItem? selectedDrive = cachedDrives.FirstOrDefault(d => driveLetter == d.Letter);

        if (selectedDrive is null)
            return OperationResult.Failure(FileError.DriveNotFound);
        
        if (!selectedDrive.IsReady)
            return OperationResult.Failure(FileError.DriveNotReady);

        currentRoot = NormalizeRoot(selectedDrive.Name);
        CurrentPath = currentRoot;
        return OperationResult.Success();
    }

    public OperationResult<OutcomeResult<ISystemEntry>> GetCurrentEntries()
    {
        if (CurrentPath == null)
            return OperationResult<OutcomeResult<ISystemEntry>>.Failure(FileError.CurrentPathNotSet);

        try
        {
            OutcomeResult<ISystemEntry> entries = directoryProvider.GetEntries(CurrentPath);
            return OperationResult<OutcomeResult<ISystemEntry>>.Success(entries);
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<OutcomeResult<ISystemEntry>>.Failure(FileError.AccessDenied);
        }
        catch (DirectoryNotFoundException)
        {
            return OperationResult<OutcomeResult<ISystemEntry>>.Failure(FileError.DirectoryNotFound);
        }
        catch (Exception)
        {
            return OperationResult<OutcomeResult<ISystemEntry>>.Failure(FileError.Unknown);
        }
    }

    public void ClearCurrentPath()
    {
        CurrentPath = null;
        currentRoot = null;
    }

    public OperationResult EnterDirectory(string entryName)
    {
        if (CurrentPath == null || currentRoot == null)
            return OperationResult.Failure(FileError.CurrentPathNotSet);

        OperationResult<string> nameResult = RelativePath.Validate(entryName); // проверка на корректность введенного пути
        if (!nameResult.IsSuccess)
            return OperationResult.Failure(nameResult.Error);
        
        string name = nameResult.Value!;
        string fullPath = Path.GetFullPath(Path.Combine(CurrentPath, name)); // нормализация пути для учета переходов на родительские уровни ".."
        
        if (!IsOnSameDrive(fullPath, currentRoot))
            return OperationResult.Failure(FileError.InvalidEntryName);

        if (File.Exists(fullPath))
            return OperationResult.Failure(FileError.NotADirectory);
        
        if (!Directory.Exists(fullPath))
            return OperationResult.Failure(FileError.EntryNotFound);

        string tempPath = CurrentPath;
        CurrentPath = fullPath;

        try
        {
            directoryProvider.GetEntries(CurrentPath);
            return OperationResult.Success();
        }
        catch (UnauthorizedAccessException)
        {
            CurrentPath = tempPath;
            return OperationResult.Failure(FileError.AccessDenied);
        }
        catch (DirectoryNotFoundException)
        {
            CurrentPath = tempPath;
            return OperationResult.Failure(FileError.DirectoryNotFound);
        }
        catch (Exception)
        {
            CurrentPath = tempPath;
            return OperationResult.Failure(FileError.Unknown);
        }
    }

    public OperationResult GoToParentDirectory()                                    // !!
    {
        if (CurrentPath is null || currentRoot is null)
            return OperationResult.Failure(FileError.CurrentPathNotSet);
        
        if (string.Equals(CurrentPath, currentRoot, StringComparison.OrdinalIgnoreCase))
            return OperationResult.Failure(FileError.AlreadyAtRoot);

        return EnterDirectory("..");
    }

    private string NormalizeRoot(string drivePath)
    {
        return Path.GetFullPath(drivePath);
    }
    
    private bool IsOnSameDrive(string path, string root)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(root))
            return false;
        
        return string.Equals(Path.GetPathRoot(path), root, StringComparison.OrdinalIgnoreCase);
    }

    public OperationResult<string> ReadTextFile(string entryName)
    {
        OperationResult<string> resultPath = ResolveTextFilePath(entryName);
        if (!resultPath.IsSuccess)
            return OperationResult<string>.Failure(resultPath.Error);
        
        string fullPath = resultPath.Value!;
        
        if (Directory.Exists(fullPath))                                             // !!
            return OperationResult<string>.Failure(FileError.IsDirectory);
        
        if (!fileProvider.Exists(fullPath))
            return OperationResult<string>.Failure(FileError.FileNotFound);

        try
        {
            long length = fileProvider.GetLength(fullPath);

            if (length > TextFilePolicy.MaxReadBytes)
                return OperationResult<string>.Failure(FileError.FileTooLarge);

            string data = fileProvider.ReadAllText(fullPath);

            return OperationResult<string>.Success(data);
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<string>.Failure(FileError.AccessDenied);
        }
        catch (Exception)
        {
            return OperationResult<string>.Failure(FileError.Unknown);
        }
    }

    public OperationResult AppendTextFile(string entryName, string text)
    {
        OperationResult<string> resultPath = ResolveTextFilePath(entryName);
        if (!resultPath.IsSuccess)
            return OperationResult.Failure(resultPath.Error);

        string fullPath = resultPath.Value!;
        
        if (Directory.Exists(fullPath))
            return OperationResult.Failure(FileError.IsDirectory);
        
        if (!fileProvider.Exists(fullPath))
            return OperationResult.Failure(FileError.FileNotFound);

        try
        {
            fileProvider.AppendAllText(fullPath, text);
            return OperationResult.Success();
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult.Failure(FileError.AccessDenied);
        }
        catch  (Exception)
        {
            return OperationResult.Failure(FileError.Unknown);
        }
    }
    
    private OperationResult<string> ResolveTextFilePath(string entryName)
    {
        if (CurrentPath is null || currentRoot is null)
            return OperationResult<string>.Failure(FileError.CurrentPathNotSet);

        OperationResult<string> resultName = TextFileName.Validate(entryName);
        if (!resultName.IsSuccess)
            return OperationResult<string>.Failure(resultName.Error);

        string fullPath = Path.GetFullPath(Path.Combine(CurrentPath, resultName.Value!));

        if (!IsOnSameDrive(fullPath, currentRoot))
            return OperationResult<string>.Failure(FileError.InvalidEntryName);
        
        return OperationResult<string>.Success(fullPath);
    }
    
    public OperationResult CreateEmptyTextFile(string entryName)
    {
        OperationResult<string> resultPath = ResolveTextFilePath(entryName);
        if (!resultPath.IsSuccess)
            return OperationResult.Failure(resultPath.Error);

        string fullPath = resultPath.Value!;

        if (Directory.Exists(fullPath))
            return OperationResult.Failure(FileError.EntryAlreadyExists);

        if (fileProvider.Exists(fullPath))
            return OperationResult.Failure(FileError.EntryAlreadyExists);

        try
        {
            fileProvider.CreateEmpty(fullPath);
            return OperationResult.Success();
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult.Failure(FileError.AccessDenied);
        }
        catch (Exception)
        {
            return OperationResult.Failure(FileError.Unknown);
        }
    }

    public OperationResult DeleteTextFile(string entryName)
    {
        OperationResult<string> resultPath = ResolveTextFilePath(entryName);
        if (!resultPath.IsSuccess)
            return OperationResult.Failure(resultPath.Error);

        string fullPath = resultPath.Value!;

        if (Directory.Exists(fullPath))
            return OperationResult.Failure(FileError.IsDirectory);

        if (!fileProvider.Exists(fullPath))
            return OperationResult.Failure(FileError.FileNotFound);

        try
        {
            fileProvider.Delete(fullPath);
            return OperationResult.Success();
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult.Failure(FileError.AccessDenied);
        }
        catch (Exception)
        {
            return OperationResult.Failure(FileError.Unknown);
        }
    }
}