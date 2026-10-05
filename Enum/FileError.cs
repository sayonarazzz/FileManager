namespace FileManager.Enum;

public enum FileError
{
    None,
    
    // Ввод (UI/Input validators) 
    EmptyInput,                 // пустая строка
    InvalidNumberFormat,        // не число (для выбора пункта)
    MenuOptionNotFound,         // число, но не из выбора пунктов
    InvalidDriveLetterFormat,   // не буква (для диска)
    
    // Диски (Core) 
    NoDrivesAvailable,          // список дисков пуст
    DriveNotFound,              // отсутствие диска по данной букве
    DriveNotReady,
    DriveReadError,             // ошибка чтения списка дисков
    DrivesNotLoaded,            // попытка работать дальше без полученных дисков
    
    // Пути и каталоги (Core/Infrastructure)
    CurrentPathNotSet,          // попытка получить путь без выбранного диска, т.е. без стартового пути (корень диска)
    DirectoryNotFound,
    AccessDenied,
    InvalidEntryName,           // имя папки: пустое/запрещённые символы
    EntryNotFound,              // нет такого элемента в текущем каталоге
    NotADirectory,              // элемент есть, но это не папка
    AlreadyAtRoot,              // попытка выйти за пределы диска (корня)
    
    // файлы (Core/Infrastructure)
    IsDirectory,
    FileNotFound,
    NotATextFile,
    FileTooLarge,
    EntryAlreadyExists,
    
    // Общее 
    Unknown                     // необработанное
}