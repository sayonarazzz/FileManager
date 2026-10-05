using FileManager.UI.Contracts;
using FileManager.UI.Menu;

namespace FileManager.UI;

public class OptionView : IOptionView
{
    public void ShowOptions(string? title, IReadOnlyList<MenuOption> options, string? currentPath = null)
    {
        Console.WriteLine();

        if (!string.IsNullOrWhiteSpace(currentPath))
        {
            Console.WriteLine(currentPath);
        }
            
        Console.WriteLine(title);

        foreach (MenuOption option in options)
        {
            Console.WriteLine($"[{option.Key}] {option.Message}");
        }
        
        Console.WriteLine();
    }
}