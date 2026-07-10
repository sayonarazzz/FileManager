namespace ConsoleFileManager.Core;

public class OperationResult<T>
{
    public bool Success { get; }
    public T?  Value { get; }

    public OperationResult(bool success, T? value)
    {
        Success = success;
        Value = value;
    }
}