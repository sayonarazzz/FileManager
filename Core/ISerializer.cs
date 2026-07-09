namespace ConsoleFileManager.Core;

public interface ISerializer<T>
{
    string Serialize(T entity);
    T Deserialize(string text);
}