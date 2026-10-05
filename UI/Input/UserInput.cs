namespace FileManager.UI.Validation;

public static class UserInput
{
    public static string? ReadLine()
    {
        Console.Write("] ");
        return Console.ReadLine();
    }
}