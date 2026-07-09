namespace ConsoleFileManager.UI;

public static class ConsoleInput
{
    public static string ReadLine(string message)
    {
        Console.WriteLine(message);
        Console.Write(" | ");
        string input = Console.ReadLine();
        return input;
    }
    
    public static string ReadLine()
    {
        Console.Write(" | ");
        string input = Console.ReadLine();
        return input;
    }
}