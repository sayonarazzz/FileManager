using ConsoleFileManager.Core;
using ConsoleFileManager.Enum;
using ConsoleFileManager.Infrastructure;
namespace ConsoleFileManager.UI;

public class ConsoleApplication
{
    private readonly IFileService<string> fileService;
    private TextDocument? currentDocument;
    private IDirectoryProvider providerDirectory;

    public ConsoleApplication(IFileService<string> fileService, IDirectoryProvider providerDirectory)
    {
        this.fileService = fileService;
        this.providerDirectory = providerDirectory;
    }

    public async Task Start()
    {
        bool exit = false;
        
        while (!exit)
        {
            ConsoleUI.ShowDirectoryInfo(FileSettings.BasePath, providerDirectory.GetDirectories(FileSettings.BasePath), providerDirectory.GetFiles(FileSettings.BasePath));
            ConsoleUI.ShowMainMenu();
            
            string choice = ConsoleInput.ReadLine();

            switch (choice)
            {
                case "9": // выход
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
                
                case "7": // перемещение
                    TryMove();
                    break;
                
                case "8": // вернуться назад по пути
                    TryBackDirectory();
                    break;
            }
        }
    }

    private async Task CreateFileAsync()
    {
        string name = ConsoleInput.ReadLine(" | Введите название файла");
        string text = ConsoleInput.ReadLine(" | Введите данные");
        string fullPath = FileSettings.GetFullPath(name);

        try
        {
            await fileService.WriteAsync(fullPath, text);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine(" | Файл не создался");
        }
        catch (IOException)
        {
            Console.WriteLine(" | Ошибка создания файла");
        }
    }

    private async Task OpenFileAsync()
    {
        string name = ConsoleInput.ReadLine(" | Введите название файла");
        string fullPath = FileSettings.GetFullPath(name);
        string text;

        try
        {
            text = await fileService.ReadAsync(fullPath);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine(" | Файл не найден");
            return;
        }
        catch (IOException)
        {
            Console.WriteLine(" | Ошибка чтения файла");
            return;
        }
        
        currentDocument = new TextDocument(fullPath, text);
        
        ConsoleUI.ShowFilePath(fullPath);
        
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
        if (currentDocument.IsModified)
        {
            ConsoleUI.ShowSaveMenu();
            
            string choice = ConsoleInput.ReadLine(" | Выберите действие");
                
            switch (choice)
            {
                case "1": // сохранить изменения
                    try
                    {
                        await fileService.WriteAsync(currentDocument.FullPath, currentDocument.Text);
                        currentDocument.IsModified = false;
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine(" | Файл не найден");
                        return false;
                    }
                    catch (IOException)
                    {
                        Console.WriteLine(" | Ошибка сохранения файла");
                        return false;
                    }

                    return true;
                    
                case "2": // не сохранять изменения
                    return true;
                    
                case "3": // отменить операцию выхода и продолжить работу с файлом
                    return false;
            }
        }

        return true;
    }

    private StatusDirectory ChangeDirectory(string path)
    {
        if (!providerDirectory.Exists(path))
        {
            return StatusDirectory.Warning;
        }
        
        FileSettings.BasePath = path;
        
        return StatusDirectory.Success;
    }

    private void TryChangeDirectory()
    {
        string name = ConsoleInput.ReadLine(" | Введите название папки");
        string path = FileSettings.GetFullPath(name);
        
        StatusDirectory status = ChangeDirectory(path);

        if (status == StatusDirectory.Success)
        {
            return;
        }
        else if (status == StatusDirectory.Warning)
        {
            if (CreateDirectory(path))
            {
                ChangeDirectory(path);
            }
        }
    }
    
    private bool CreateDirectory(string fullPath)
    {
        Console.WriteLine(" | Такой папки не существует");
        ConsoleUI.ShowQuestionMenu(" | Создать такую папку?");
        
        string choice = ConsoleInput.ReadLine(" | Выберите действие:");

        if (choice == "1")
        {
            bool status = TryCreateDirectory(fullPath);
            
            while (!status)
            {
                ConsoleUI.ShowQuestionMenu(" | Папка не создалась. Попробовать снова?");
                
                string text = ConsoleInput.ReadLine();

                if (text == "1")
                {
                    status = TryCreateDirectory(fullPath);
                }
                else if (text != "1") // как только сделаем валидацию ввода, так сразу жеско == "2".
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private bool TryCreateDirectory(string fullPath)
    {
        try
        {
            providerDirectory.CreateDirectory(fullPath);
        }
        catch  (IOException)
        {
            Console.WriteLine(" | Не удалось создать папку");
            return false;
        }
        
        return true;
    }

    private void TryBackDirectory()
    {
        if (FileSettings.BasePath == FileSettings.RootPath)
        {
            Console.WriteLine(" | Выйти дальше этой корневой папки нельзя");
            return;
        }

        string? parentPath = providerDirectory.GetParentDirectory(FileSettings.BasePath);

        if (parentPath == null)
        {
            Console.WriteLine(" | Не удалось перейти в корневую папку");
            return;
        }

        StatusDirectory status = ChangeDirectory(parentPath);

        if (status == StatusDirectory.Warning)
        {
            Console.WriteLine(" | Ошибка перехода");
        }
    }

    private void TryChangeAbsoluteDirectory()
    {
        string fullPath = ConsoleInput.ReadLine(" | Введите абсолютный путь");

        if (!FileSettings.IsValidPath(fullPath))
        {
            Console.WriteLine(" | Выход за пределы рабочей директории не возможен");
            return;
        }
        
        StatusDirectory status = ChangeDirectory(fullPath);
        
        if (status == StatusDirectory.Warning)
        {
            Console.WriteLine(" | Такого пути не существует");
        }
    }

    private void TryDelete()
    {
        string name = ConsoleInput.ReadLine(" | Введите название папки или файла");
        string path = FileSettings.GetFullPath(name);
        
        ItemType type = providerDirectory.GetItemType(path);

        if (type == ItemType.None)
        {
            Console.WriteLine(" | Нет такого файла или папки");
            return;
        }
        
        ConsoleUI.ConfirmDelete();
                
        string choice = ConsoleInput.ReadLine();

        if (choice != "1")
        {
            return;
        }
        
        switch (type)
        {
            case ItemType.File:
                try
                {
                    providerDirectory.DeleteFile(path);
                    Console.WriteLine(" | Файл успешно удален");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для удаления");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Удаление отменено");
                }
                
                break;
            
            case ItemType.Directory:
                try
                {
                    providerDirectory.DeleteDirectory(path);
                    Console.WriteLine(" | Папка успешно удалена");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для удаления");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Удаление отменено");
                }
                
                break;
        }
    }

    private void TryRename()
    {
        string oldName = ConsoleInput.ReadLine(" | Введите название обьекта, который хотите переименовать");
        string oldPath = FileSettings.GetFullPath(oldName);
        
        ItemType type = providerDirectory.GetItemType(oldPath);
        
        if (type == ItemType.None)
        {
            Console.WriteLine(" | Нет такого файла или папки");
            return;
        }
        
        string newName = ConsoleInput.ReadLine(" | Введите новое название");
        string newPath = FileSettings.GetFullPath(newName);
        
        ConsoleUI.ConfirmRename();
                
        string choice = ConsoleInput.ReadLine();

        if (choice != "1")
        {
            return;
        }
        
        switch (type)
        {
            case ItemType.File:
                try
                {
                    providerDirectory.RenameFile(oldPath, newPath);
                    Console.WriteLine(" | Файл успешно переименован");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для переименования");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Переименование отменено");
                }
                
                break;
            
            case ItemType.Directory:
                try
                {
                    providerDirectory.RenameDirectory(oldPath, newPath);
                    
                    if (oldPath == FileSettings.BasePath)
                    {
                        FileSettings.BasePath = newPath;
                    }
                    
                    Console.WriteLine(" | Папка успешно переименованна");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для переименования");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Переименование отменено");
                }
                
                break;
        }
    }

    private void TryMove()
    {
        string name = ConsoleInput.ReadLine(" | Введите название обьекта, который хотите переместить");
        string oldPath = FileSettings.GetFullPath(name);
        
        ItemType type = providerDirectory.GetItemType(oldPath);

        if (type == ItemType.None)
        {
            Console.WriteLine(" | Нет такого файла или папки");
            return;
        }

        if (oldPath == FileSettings.BasePath)
        {
            Console.WriteLine(" | Перемещение текущей папки пока невозможно");
            return;
        }
        
        string tempPath = ConsoleInput.ReadLine(" | Введите путь, куда хотите переместить обьект");

        if (!FileSettings.IsValidPath(tempPath))
        {
            Console.WriteLine(" | Перемещение вне корневой папки невозможно");
            return;
        }
        
        if (!providerDirectory.Exists(tempPath))
        {
            Console.WriteLine(" | Такой папки не существует");
            return;
        }
        
        string newPath = FileSettings.GetFullPath(tempPath, name);
        
        if (providerDirectory.GetItemType(newPath) != ItemType.None)
        {
            Console.WriteLine(" | В папке назначения уже существует объект с таким именем");
            return;
        }
        
        if (oldPath == newPath)
        {
            Console.WriteLine(" | Объект уже находится в этой папке");
            return;
        }
        
        ConsoleUI.ConfirmMove();
                
        string choice = ConsoleInput.ReadLine();

        if (choice != "1")
        {
            return;
        }
        
        switch (type)
        {
            case ItemType.File:
                try
                {
                    providerDirectory.MoveFile(oldPath, newPath);
                    Console.WriteLine(" | Файл успешно перемещен");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для перемещения");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Перемещение отменено");
                }
                
                break;
            
            case ItemType.Directory:
                try
                {
                    providerDirectory.MoveDirectory(oldPath, newPath);
                    
                    Console.WriteLine(" | Папка успешно перемещена");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для перемещения");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Перемещение отменено");
                }
                
                break;
        }
    }

    private void TryCopy()
    {
        string name = ConsoleInput.ReadLine(" | Введите название обьекта, который хотите скопировать");
        string oldPath = FileSettings.GetFullPath(name);
        
        ItemType type = providerDirectory.GetItemType(oldPath);

        if (type == ItemType.None)
        {
            Console.WriteLine(" | Нет такого файла или папки");
            return;
        }

        if (type == ItemType.Directory)
        {
            Console.WriteLine(" | Копирование папки не поддерживается");
            return;
        }

        if (oldPath == FileSettings.BasePath)
        {
            Console.WriteLine(" | Копирование текущей папки пока невозможно");
            return;
        }
        
        string tempPath = ConsoleInput.ReadLine(" | Введите путь, куда хотите копировать обьект");

        if (!FileSettings.IsValidPath(tempPath))
        {
            Console.WriteLine(" | Копирование вне корневой папки невозможно");
            return;
        }
        
        if (!providerDirectory.Exists(tempPath))
        {
            Console.WriteLine(" | Такой папки не существует");
            return;
        }
        
        string newPath = FileSettings.GetFullPath(tempPath, name);
        
        if (providerDirectory.GetItemType(newPath) != ItemType.None)
        {
            Console.WriteLine(" | В папке назначения уже существует объект с таким именем");
            return;
        }
        
        if (oldPath == newPath)
        {
            Console.WriteLine(" | Объект уже находится в этой папке");
            return;
        }
        
        ConsoleUI.ConfirmCopy();
                
        string choice = ConsoleInput.ReadLine();
        
        if (choice != "1")
        {
            return;
        }
        
        switch (type)
        {
            case ItemType.File:
                try
                {
                    providerDirectory.CopyFile(oldPath, newPath);
                    Console.WriteLine(" | Файл успешно скопирован");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для копирования");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Копирование отменено");
                }
                
                break;
            
            case ItemType.Directory:
                try
                {
                    providerDirectory.CopyDirectory(oldPath, newPath);
                    
                    Console.WriteLine(" | Папка успешно скопирована");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine(" | Копирование папок пока не поддерживается");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(" | Недостаточно прав для копирования");
                }
                catch (IOException)
                {
                    Console.WriteLine(" | Произошла ошибка. Копирование отменено");
                }
                
                break;
        }
    }
}
