namespace FileManager.UI.Menu;

public static class BrowseMenu
{
    public static IReadOnlyList<MenuOption> Options { get; } =
    [
        new MenuOption(1, "Открыть папку"),
        new MenuOption(2, "Назад"),
        new MenuOption(3, "Смена диска"),
        new MenuOption(4, "Открыть текстовый файл .txt"),
        new MenuOption(5, "Создать пустой .txt"),
        new MenuOption(6, "Удалить .txt"),
        new MenuOption(0, "Выход"),
    ];
}