using FileManager.UI.Menu;

namespace FileManager.UI.Contracts;

public interface IOptionView
{
    void ShowOptions(string? title, IReadOnlyList<MenuOption> options, string? currentPath = null);
}