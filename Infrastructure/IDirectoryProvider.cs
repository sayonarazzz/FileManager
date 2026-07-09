using ConsoleFileManager.Enum;
namespace ConsoleFileManager.Infrastructure;

public interface IDirectoryProvider
{
    bool Exists(string path);
    void CreateDirectory(string path);
    string? GetParentDirectory(string path);
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
    ItemType GetItemType(string path);
    void DeleteDirectory(string path);
    void DeleteFile(string path);
    void RenameDirectory(string oldPath, string newPath);
    void RenameFile(string oldPath, string newPath);
    void MoveDirectory(string oldPath, string newPath);
    void MoveFile(string oldPath, string newPath);
    void CopyFile(string oldPath, string newPath);
    void CopyDirectory(string oldPath, string newPath);
}