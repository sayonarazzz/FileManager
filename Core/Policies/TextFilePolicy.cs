namespace FileManager.Core.Policies;

public static class TextFilePolicy
{
    public const long MaxReadBytes = 2L * 1024 * 1024; // на чтение допустимо мах 2 MB 
    
    public static bool IsTextFile(string name) =>   
        name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) 
        && !name.Equals(".txt", StringComparison.OrdinalIgnoreCase);
}