using FileManager.Enum;

namespace FileManager.Core.Input;

public static class EntryName
{
    public static OperationResult<string> Validate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return OperationResult<string>.Failure(FileError.EmptyInput);
        
        string name = input.Trim();
        
        if (name is "." or "..")
            return OperationResult<string>.Failure(FileError.InvalidEntryName);
        
        if (name.EndsWith('.') || name.EndsWith(' '))                           
            return OperationResult<string>.Failure(FileError.InvalidEntryName);
        
        if (name.Contains('\\') || name.Contains('/'))                         
            return OperationResult<string>.Failure(FileError.InvalidEntryName);
        
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)              // Запрещённые символы имени файла
            return OperationResult<string>.Failure(FileError.InvalidEntryName);

        return OperationResult<string>.Success(name);
    }
}