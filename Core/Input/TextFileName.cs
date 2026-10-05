using FileManager.Enum;

namespace FileManager.Core.Input;

public static class TextFileName
{
    private const string Suffix = ".txt";

    public static OperationResult<string> Validate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return OperationResult<string>.Failure(FileError.EmptyInput);
        
        string baseName = input.Trim();

        if (baseName.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase) && baseName.Length > Suffix.Length)
            baseName = baseName[..^Suffix.Length];

        if (string.IsNullOrWhiteSpace(baseName))
            return OperationResult<string>.Failure(FileError.EmptyInput);

        OperationResult<string> resultBase = EntryName.Validate(baseName);
        if (!resultBase.IsSuccess)
            return OperationResult<string>.Failure(resultBase.Error);

        baseName = resultBase.Value!;
        string fileName = baseName + Suffix;

        return OperationResult<string>.Success(fileName);
    }
}