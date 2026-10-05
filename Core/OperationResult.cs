using FileManager.Enum;
namespace FileManager.Core;

public class OperationResult
{
    public FileError Error { get; } 
    public bool IsSuccess => Error == FileError.None;
    
    protected OperationResult(FileError error)
    {
        Error = error;
    }
    
    public static OperationResult Success() => new(FileError.None);
    public static OperationResult Failure(FileError error) => new(error);
}

public class OperationResult<T> : OperationResult
{
    public T? Value { get; }
    
    private OperationResult(T value) : base(FileError.None)
    {
        Value = value;
    }

    private OperationResult(FileError error) : base(error)
    {
        Value = default;
    }

    public static OperationResult<T> Success(T value) => new(value);
    public static OperationResult<T> Failure(FileError error) => new(error);
}