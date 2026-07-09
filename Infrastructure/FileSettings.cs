namespace ConsoleFileManager.Infrastructure;

public static class FileSettings
{
    public static string RootPath { get; } = @"C:\Proga";
    public static string BasePath { get; set; } = RootPath;

    public static string GetFullPath(string name)
    {
        return Path.Combine(BasePath, name);
    }
    
    public static string GetFullPath(string oldPath, string name)
    {
        return Path.Combine(oldPath, name);
    }
    
    public static bool IsValidPath(string path)
    {
        return path.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase); // проверяет корневую папку (второй параметр просто игнорирует регистр)
    }
}