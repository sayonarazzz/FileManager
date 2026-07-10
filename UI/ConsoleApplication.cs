using ConsoleFileManager.Core;
using ConsoleFileManager.Enum;
namespace ConsoleFileManager.UI;

public class ConsoleApplication
{
    private readonly IFileSystemService systemService;
    private Document? currentDocument;
    
    public ConsoleApplication(IFileSystemService systemService)
    {
        this.systemService = systemService;
    }

    public async Task Start()
    {
        bool exit = false;
        
        while (!exit)
        {
            ConsoleUI.ShowDirectoryInfo(systemService.GetCurrentPath(), systemService.GetDirectories(), systemService.GetFiles());
            ConsoleUI.ShowMainMenu();
            
            string choice = ConsoleInput.ReadLine();

            switch (choice)
            {
                case "8": // выход
                    exit = true;
                    break;

                case "1": // открыть файл (просто найти и сохранить содержимое)
                    await OpenFileAsync();
                    break;

                case "2": // создать файл
                    await CreateFileAsync();
                    break;

                case "3": // смена дирректории (ее создание при необходимости)
                    TryChangeDirectory();
                    break;
                
                case "4": // переход по абсолютному пути
                    TryChangeAbsoluteDirectory();
                    break;
                
                case "5": // удаление
                    TryDelete();
                    break;
                
                case "6": // переименование
                    TryRename(); 
                    break;
                
                case "7": // вернуться назад по пути
                    TryBackDirectory();
                    break;
            }
        }
    }

    private async Task CreateFileAsync()
    {
        string name = ConsoleInput.ReadLine(" | Введите название файла");
        string text = ConsoleInput.ReadLine(" | Введите данные");
        
        await systemService.TryCreateFileAsync(name, text);
    }

    private async Task OpenFileAsync()
    {
        string name = ConsoleInput.ReadLine(" | Введите название файла");

        OperationResult<string> result = await systemService.TryOpenFileAsync(name);

        if (!result.Success)
        {
            Console.WriteLine(" | Не удалось открыть файл");
            return;
        }

        currentDocument = new Document(name, result.Value);

        ConsoleUI.ShowFilePath(currentDocument.FullPath);

        await FileMenuAsync();
    }

    private async Task FileMenuAsync()
    {
        bool exit = false;

        while (!exit)
        {
            ConsoleUI.ShowDocumentMenu();
            
            string choice = ConsoleInput.ReadLine(" | Выберите действие");
            
            switch (choice)
            {
                case "1": // посмотреть содержимое
                    ConsoleUI.ShowInfo(currentDocument.Text);
                    break;
                
                case "2": // написать новые данные без сохранения
                    string newText = " " + ConsoleInput.ReadLine(" | Введите текст");
                    
                    currentDocument.Text += newText;
                    currentDocument.IsModified = true;
                    
                    break;
                
                case "3": // выход с сохранение (если нужно)
                    if (await CloseDocumentAsync())
                    {
                        currentDocument = null;
                        exit = true;
                    }
                    
                    break;
            }
        }
    }

    private async Task<bool> CloseDocumentAsync()
    {
        if (!currentDocument.IsModified)
        {
            return true;
        }

        ConsoleUI.ShowSaveMenu();

        string choice = ConsoleInput.ReadLine(" | Выберите действие");

        switch (choice)
        {
            case "1":
                bool success = await systemService.TryAppendFileAsync(currentDocument.FullPath, currentDocument.Text);

                if (!success)
                {
                    Console.WriteLine(" | Не удалось сохранить файл");
                    return false;
                }

                currentDocument.IsModified = false;
                
                return true;

            case "2":
                return true;

            case "3":
                return false;

            default:
                return false;
        }
    }

    private void TryChangeDirectory()
    {
        string name = ConsoleInput.ReadLine(" | Введите название папки");

        if (systemService.TryGoToDirectory(name))
        {
            return;
        }

        Console.WriteLine(" | Такой папки не существует");
        ConsoleUI.ShowQuestionMenu(" | Создать такую папку?");

        string choice = ConsoleInput.ReadLine(" | Выберите действие:");

        if (choice != "1")
        {
            return;
        }

        while (true)
        {
            if (systemService.TryCreateDirectory(name))
            {
                systemService.TryGoToDirectory(name);
                return;
            }

            ConsoleUI.ShowQuestionMenu(" | Папка не создалась. Попробовать снова?");

            choice = ConsoleInput.ReadLine(" | Выберите действие:");

            if (choice != "1")
            {
                return;
            }
        }
    }
    
    private bool CreateDirectory(string name)
    {
        Console.WriteLine(" | Такой папки не существует");
        ConsoleUI.ShowQuestionMenu(" | Создать такую папку?");

        string choice = ConsoleInput.ReadLine(" | Выберите действие:");

        if (choice != "1")
        {
            return false;
        }

        while (!systemService.TryCreateDirectory(name))
        {
            ConsoleUI.ShowQuestionMenu(" | Папка не создалась. Попробовать снова?");

            choice = ConsoleInput.ReadLine(" | Выберите действие:");

            if (choice != "1")
            {
                return false;
            }
        }

        return true;
    }

    private void TryBackDirectory()
    {
        if (!systemService.TryBackDirectory())
        {
            Console.WriteLine(" | Не удалось перейти в родительскую директорию");
        }
    }

    private void TryChangeAbsoluteDirectory()
    {
        string path = ConsoleInput.ReadLine(" | Введите абсолютный путь");

        if (!systemService.TryGoToAbsolutePath(path))
        {
            Console.WriteLine(" | Не удалось перейти по указанному пути");
        }
    }

    private void TryDelete()
    {
        string name = ConsoleInput.ReadLine(" | Введите название папки или файла");

        ConsoleUI.ConfirmDelete();

        string choice = ConsoleInput.ReadLine();

        if (choice != "1")
        {
            return;
        }

        if (systemService.TryDelete(name))
        {
            Console.WriteLine(" | Объект успешно удален");
        }
        else
        {
            Console.WriteLine(" | Не удалось удалить объект");
        }
    }

    private void TryRename()
    {
        string oldName = ConsoleInput.ReadLine(" | Введите название объекта, который хотите переименовать");
        string newName = ConsoleInput.ReadLine(" | Введите новое название");

        ConsoleUI.ConfirmRename();

        string choice = ConsoleInput.ReadLine();

        if (choice != "1")
        {
            return;
        }

        if (systemService.TryRename(oldName, newName))
        {
            Console.WriteLine(" | Объект успешно переименован");
        }
        else
        {
            Console.WriteLine(" | Не удалось переименовать объект");
        }
    }
}
