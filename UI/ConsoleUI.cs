namespace ConsoleFileManager.UI;

public static class ConsoleUI
{
    private const int Count = 3; // кол-во элементов, выводимых в строку
    
    public static void ShowMainMenu()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Выбери действие:");
        Console.WriteLine(" | 1: Загрузить файл");
        Console.WriteLine(" | 2: Создать файл");
        Console.WriteLine(" | 3: Сменить рабочую папку");
        Console.WriteLine(" | 4: Выполнить переход по абсолютному пути");
        Console.WriteLine(" | 5: Удалить");
        Console.WriteLine(" | 6: Переименовать");
        Console.WriteLine(" | 7: Переместить");
        Console.WriteLine(" | 8: Вернуться назад");
        Console.WriteLine(" | 9: Выход");
        Console.WriteLine(" | ");
    }

    public static void ShowDocumentMenu()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Выберите действие");
        Console.WriteLine(" | 1: Посмотреть содержимое");
        Console.WriteLine(" | 2: Дописать данные");
        Console.WriteLine(" | 3: Закрыть документ");
        Console.WriteLine(" | ");
    }

    public static void ShowSaveMenu()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Выберите действие");
        Console.WriteLine(" | 1: Сохранить");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | 3: Отмена");
        Console.WriteLine(" | ");
    }

    public static void ShowQuestionMenu(string message)
    {
        Console.WriteLine(" | ");
        Console.WriteLine(message);
        Console.WriteLine(" | 1: Да");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | ");
    }
    
    public static void ShowInfo(string info)
    {
        Console.Write(" | ");
        Console.WriteLine(info);
    }

    public static void ShowDirectoryPath(string path)
    {
        ShowPath(path, "Текущая папка");
    }

    public static void ShowFilePath(string path)
    {
        ShowPath(path, "Текущий файл");
    }

    public static void ShowPath(string path, string message)
    {
        Console.WriteLine(" | ");
        Console.WriteLine($" | {message}:");
        Console.WriteLine(" | " + path);
        Console.WriteLine(" | ");
    }
    
    public static void ShowDirectoryInfo(string path, string[] directories, string[] files)
    {
        ShowDirectoryPath(path);
        ShowDirectoryContent(directories, files);
    }

    public static void ShowDirectoryContent(string[] directories, string[] files)
    {
        Console.WriteLine(" |================================================|");
        ShowItems("Папки", "[D]", directories);
        Console.WriteLine(" | ");
        ShowItems("Файлы", "[F]", files);
        Console.WriteLine(" |================================================|");
    }

    public static void ShowItems(string name, string icon, string[] items)
    {
        if (items.Length == 0)
        {
            Console.WriteLine(" | " + name + " отсутствуют");
            return;
        }

        Console.WriteLine($" | {name}:");
        Console.Write(" | ");

        for (int i = 0; i < items.Length; i++)
        {
            Console.Write(icon + " " + items[i]);

            if (i != items.Length - 1)
            {
                Console.Write(", ");
            }

            if ((i + 1) % Count == 0 && i != items.Length - 1)
            {
                Console.WriteLine();
                Console.Write(" | ");
            }
        }
        
        Console.WriteLine();
    }

    public static void ConfirmDelete()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Уверены, что хотите удалить?");
        Console.WriteLine(" | 1: Да");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | ");
    }
    
    public static void ConfirmRename()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Уверены, что хотите переименовать?");
        Console.WriteLine(" | 1: Да");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | ");
    }
    
    public static void ConfirmMove()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Уверены, что хотите переместить?");
        Console.WriteLine(" | 1: Да");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | ");
    }

    public static void ConfirmCopy()
    {
        Console.WriteLine(" | ");
        Console.WriteLine(" | Уверены, что хотите копировать?");
        Console.WriteLine(" | 1: Да");
        Console.WriteLine(" | 2: Нет");
        Console.WriteLine(" | ");
    }
}