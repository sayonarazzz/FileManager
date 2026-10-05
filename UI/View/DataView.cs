using FileManager.Core.Models;
using FileManager.UI.Contracts;
using FileManager.Core.Contracts;
using FileManager.Enum;

namespace FileManager.UI;

public class DataView : IDataView
{
    public void ShowDrives(IReadOnlyList<DriveItem> drives)
    {
        Console.WriteLine("Доступные диски:");
        
        foreach (DriveItem drive in drives)
        {
            string label = string.IsNullOrWhiteSpace(drive.Label) ? "Без метки" : drive.Label;
            string status = drive.IsReady ? "Готов" : "Не готов";
            Console.WriteLine($"{drive.Name} [{label}] ({drive.DriveType}, {status})");
        }
        
        Console.WriteLine();
    }

    public void ShowPartialDrives(int count, IReadOnlyList<DriveItem> partialDrives)
    {
        if (count <= 0)
            return;
        
        Console.WriteLine($"Не удалось прочитать полностью [{count}]");

        foreach (DriveItem drive in partialDrives)
        {
            Console.WriteLine($"{drive.Name}");
        }
        
        Console.WriteLine();
    }

    public void ShowSelectedDrive(string drivePath)
    {
        Console.WriteLine();
        Console.WriteLine($"Выбран диск {drivePath}");
        Console.WriteLine();
    }

    public void ShowEntries(IReadOnlyList<ISystemEntry> entries)
    {
        Console.WriteLine("Содержимое:");

        if (entries.Count == 0)
        {
            Console.WriteLine(" пусто");
            Console.WriteLine();
            return;
        }

        foreach (ISystemEntry entry in entries)
        {
            if (entry is DirectoryItem)
            {
                Console.WriteLine($" [папка] {entry.Name}");
            }
            else if (entry is FileItem file)
            {
                Console.WriteLine($" {file.Name} [{FormatSize(file.Size)}]");
            }
            else
            {
                Console.WriteLine(" ??");
            }
        }
        
        Console.WriteLine();
    }

    public void ShowPartialEntries(int count, IReadOnlyList<ISystemEntry> partialEntries)
    {
        if (count <= 0)
            return;
        
        Console.WriteLine($"Не удалось полностью прочитать [{count}]");

        foreach (ISystemEntry entry in partialEntries)
        {
            Console.WriteLine($"{entry.Name}");
        }
        
        Console.WriteLine();
    }
    
    public void ShowTextFileContent(string fileName, string data)
    {
        Console.WriteLine();
        Console.WriteLine($"Файл: {fileName}");
        Console.WriteLine();
        if (data.Length == 0)
            Console.WriteLine("(пусто)");
        else
            Console.Write(data);
        Console.WriteLine();
    }

    public void ShowError(FileError error)
    {
        if (error == FileError.None)
            return;
        
        string message = error switch
        {
            FileError.EmptyInput => "Ввод не может быть пустым.",
            FileError.InvalidNumberFormat => "Введите число.",
            FileError.MenuOptionNotFound => "Такого пункта меню нет.",
            FileError.InvalidDriveLetterFormat => "Введите одну букву латинского алфавита.",
            FileError.DrivesNotLoaded => "Список дисков не загружен.",
            FileError.NoDrivesAvailable => "Нет доступных дисков.",
            FileError.DriveReadError => "Не удалось получить список дисков.",
            FileError.DriveNotFound => "Диск с такой буквой не найден.",
            FileError.DriveNotReady => "Диск не готов к работе.",
            FileError.CurrentPathNotSet => "Сначала выберите диск.",
            FileError.AccessDenied => "Нет доступа.",
            FileError.DirectoryNotFound => "Путь не найден.",
            FileError.Unknown => "Неизвестная ошибка.",
            FileError.InvalidEntryName => "Недопустимое имя.",
            FileError.EntryNotFound => "Папка с таким именем не найдена.",
            FileError.NotADirectory => "Это не папка. Открыть можно только папку.",
            FileError.AlreadyAtRoot => "Вы уже в корне диска. Для его смены выберете пункт [3]",
            FileError.IsDirectory => "Указано имя папки, а не файла.",
            FileError.FileNotFound => "Файл с таким именем не найден.",
            FileError.NotATextFile => "Не поддерживается открытие нетекстовых файлов. Допустимы только файлы .txt.",
            FileError.FileTooLarge => "Файл слишком большой для просмотра (лимит 2 МБ).",
            FileError.EntryAlreadyExists => "Файл или папка с таким именем уже существует.",
            _ => "Необработанная ошибка."
        };
        
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }
    
    private static string FormatSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024L * 1024 * 1024)
            return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
    }
}