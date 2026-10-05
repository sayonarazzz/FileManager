using FileManager.Enum;

namespace FileManager.Core;

public static class RelativePath
{
    private static readonly char[] ForbiddenChars = BuildForbiddenChars();

    private static char[] BuildForbiddenChars()
    {
        HashSet<char> chars = new(Path.GetInvalidPathChars()); // коллекция символов, которые по умолчанию ОС запрещенны для пути
        chars.Remove('\\');                                            // однако я предполагаю резрешить / и \
        chars.Remove('/');
        return chars.ToArray();
    }

    public static OperationResult<string> Validate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))                           // ввод пустой (null/"")
            return OperationResult<string>.Failure(FileError.EmptyInput);

        string name = input.Trim().Replace('/', '\\');                  // меняем / на \ (мы дали ввести /)

        if (Path.IsPathRooted(name))                                    // защита при Combine - если второй аргумент - абсолютный путь
            return OperationResult<string>.Failure(FileError.InvalidEntryName); // с новым корнем, то корень поменяется (избегаем этого)

        if (name.IndexOfAny(ForbiddenChars) >= 0)
            return OperationResult<string>.Failure(FileError.InvalidEntryName);

        return OperationResult<string>.Success(name);
    }
}