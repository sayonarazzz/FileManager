using FileManager.Core;
using FileManager.Core.Contracts;
using FileManager.Core.Models;
using FileManager.Core.Input;
using FileManager.UI.Contracts;
using FileManager.UI.Menu;
using FileManager.UI.Validation;

namespace FileManager.UI;

public class Application
{
    private readonly ICentralSystem centralSystem;
    private readonly IDataView dataView;
    private readonly IOptionView optionView;

    public Application(ICentralSystem centralSystem, IDataView dataView, IOptionView optionView)
    {
        this.centralSystem = centralSystem;
        this.dataView = dataView;
        this.optionView = optionView;
    }

    public void Start()
    {
        if (!TryShowDrives())
            return;
        
        RunMainMenu();
    }
    private bool TryShowDrives()
    {
        OperationResult<OutcomeResult<DriveItem>> operation = centralSystem.GetDrives();

        if (!operation.IsSuccess)
        {
            dataView.ShowError(operation.Error);
            return false;
        }

        OutcomeResult<DriveItem> drives = operation.Value!; // вернись сюда и подумай
        dataView.ShowDrives(drives.Items);

        if (drives.HasPartial)
        {
            dataView.ShowPartialDrives(drives.PartialCount, drives.PartialItems);
        }

        return true;
    }

    private void RunMainMenu()
    {
        while (true)
        {
            optionView.ShowOptions("Выберете дальнейшее действие:", MainMenu.Options);
            
            string? choice = UserInput.ReadLine();
            OperationResult<int> choiceResult = MenuValidator.TryParseChoiceToNumber(choice, MainMenu.Options);
            
            if (!choiceResult.IsSuccess)
            {
                dataView.ShowError(choiceResult.Error);
                continue;
            }
            
            switch (choiceResult.Value)
            {
                case 0:
                    return;
                
                case 1:
                    if (RunSelectDrive())
                    {
                        if (!RunBrowseLoop())
                            return;
                    }
                    break;
            }
        }
    }

    private bool RunSelectDrive()
    {
        while (true)
        {
            Console.WriteLine("Введите букву диска");
            
            string? input = UserInput.ReadLine();
            OperationResult<char> inputResult = DriveLetterValidator.TryParseDriveLetter(input);

            if (!inputResult.IsSuccess)
            {
                dataView.ShowError(inputResult.Error);
                continue;
            }
            
            OperationResult selectResult = centralSystem.SelectDriveByLetter(inputResult.Value);

            if (!selectResult.IsSuccess)
            {
                dataView.ShowError(selectResult.Error);
                continue;
            }
            
            dataView.ShowSelectedDrive(centralSystem.CurrentPath!); // вернись сюда и подумай
            
            return true;
        }
    }

    private void ShowCurrentDirectory()
    {
        OperationResult<OutcomeResult<ISystemEntry>> operation = centralSystem.GetCurrentEntries();

        if (!operation.IsSuccess)
        {
            dataView.ShowError(operation.Error);
            return;
        }
        
        OutcomeResult<ISystemEntry> entries = operation.Value!; // вернись сюда и подумай
        dataView.ShowEntries(entries.Items);

        if (entries.HasPartial)
        {
            dataView.ShowPartialEntries(entries.PartialCount, entries.PartialItems);
        }
    }
    
    private bool RunBrowseLoop()
    {
        while (true)
        {
            ShowCurrentDirectory();
            optionView.ShowOptions("Действия:", BrowseMenu.Options, centralSystem.CurrentPath);
            
            string? choice = UserInput.ReadLine();
            OperationResult<int> choiceResult = MenuValidator.TryParseChoiceToNumber(choice, BrowseMenu.Options);
            
            if (!choiceResult.IsSuccess)
            {
                dataView.ShowError(choiceResult.Error);
                continue;
            }
            
            switch (choiceResult.Value)
            {
                case 0:
                    return false;
                
                case 1:
                    RunOpenFolder();
                    break;
                
                case 2:
                    OperationResult backResult = centralSystem.GoToParentDirectory();
                    
                    if (!backResult.IsSuccess)
                        dataView.ShowError(backResult.Error);
                    break;
                
                case 3:
                    centralSystem.ClearCurrentPath();
                    
                    if (!TryShowDrives())
                        return false;
                    return true;
                
                case 4:
                    RunTextFile();
                    break;
                
                case 5:
                    RunCreateTextFile();
                    break;

                case 6:
                    RunDeleteTextFile();
                    break;
            }
        }
    }
    private void RunOpenFolder()
    {
        Console.WriteLine("Введите имя папки или путь папки:");
        
        string? input = UserInput.ReadLine();
        OperationResult<string> nameResult = RelativePath.Validate(input);
        
        if (!nameResult.IsSuccess)
        {
            dataView.ShowError(nameResult.Error);
            return;
        }
        
        OperationResult enterResult = centralSystem.EnterDirectory(nameResult.Value!);
        
        if (!enterResult.IsSuccess)
        {
            dataView.ShowError(enterResult.Error);
        }
    }
    
    private void RunTextFile()
    {
        Console.WriteLine("Введите имя файла без расширения:");

        string? nameInput = UserInput.ReadLine();
        OperationResult<string> nameResult = TextFileName.Validate(nameInput);

        if (!nameResult.IsSuccess)
        {
            dataView.ShowError(nameResult.Error);
            return;
        }

        string fileName = nameResult.Value!;

        OperationResult<string> readResult = centralSystem.ReadTextFile(fileName);
        if (!readResult.IsSuccess)
        {
            dataView.ShowError(readResult.Error);
            return;
        }

        dataView.ShowTextFileContent(fileName, readResult.Value!);

        Console.WriteLine("Можно дописать данные в конец (Enter — не менять файл):");

        string? appendInput = UserInput.ReadLine();
        if (string.IsNullOrEmpty(appendInput))
            return;

        OperationResult appendResult = centralSystem.AppendTextFile(fileName, appendInput);
        if (!appendResult.IsSuccess)
            dataView.ShowError(appendResult.Error);
    }

    private void RunCreateTextFile()
    {
        Console.WriteLine("Введите имя нового файла:");

        string? input = UserInput.ReadLine();
        OperationResult<string> nameResult = TextFileName.Validate(input);

        if (!nameResult.IsSuccess)
        {
            dataView.ShowError(nameResult.Error);
            return;
        }

        OperationResult createResult = centralSystem.CreateEmptyTextFile(nameResult.Value!);
        if (!createResult.IsSuccess)
            dataView.ShowError(createResult.Error);
    }

    private void RunDeleteTextFile()
    {
        Console.WriteLine("Введите имя файла для удаления:");

        string? input = UserInput.ReadLine();
        OperationResult<string> nameResult = TextFileName.Validate(input);

        if (!nameResult.IsSuccess)
        {
            dataView.ShowError(nameResult.Error);
            return;
        }

        OperationResult deleteResult = centralSystem.DeleteTextFile(nameResult.Value!);
        if (!deleteResult.IsSuccess)
            dataView.ShowError(deleteResult.Error);
    }
}