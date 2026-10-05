using FileManager.Core;
using FileManager.Enum;

namespace FileManager.UI.Validation;

public static class DriveLetterValidator
{
    public static OperationResult<char> TryParseDriveLetter(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return OperationResult<char>.Failure(FileError.EmptyInput);
        }
        
        string trimmed = input.Trim();

        if (trimmed.Length != 1 || !char.IsAsciiLetter(trimmed[0]))
        {
            return OperationResult<char>.Failure(FileError.InvalidDriveLetterFormat);
        }
        
        return OperationResult<char>.Success(char.ToUpperInvariant(trimmed[0]));
    }
}