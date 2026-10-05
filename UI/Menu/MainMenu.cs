namespace FileManager.UI.Menu;

public static class MainMenu
{
    public static IReadOnlyList<MenuOption> Options { get; } =
    [
        new MenuOption(1, "Выберете диск"),
        new MenuOption(0, "Выход")
    ];
}