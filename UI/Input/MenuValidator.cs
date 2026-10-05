using FileManager.Core;
using FileManager.Enum;
using FileManager.UI.Menu;
namespace FileManager.UI.Validation;

public static class MenuValidator
{
    public static OperationResult<int> TryParseChoiceToNumber(string? input, IReadOnlyList<MenuOption> menu)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return OperationResult<int>.Failure(FileError.EmptyInput);
        }

        if (!int.TryParse(input.Trim(), out int key))
        {
            return OperationResult<int>.Failure(FileError.InvalidNumberFormat);
        }

        if (!menu.Any(d => d.Key == key))
        {
            return OperationResult<int>.Failure(FileError.MenuOptionNotFound);
        }
        
        return OperationResult<int>.Success(key);
    }
}