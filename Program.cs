using FileManager.Core;
using FileManager.Infrastructure;
using FileManager.UI;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            DriveProvider driveProvider = new DriveProvider();
            DirectoryProvider directoryProvider = new DirectoryProvider();
            FileProvider fileProvider = new FileProvider();
            CentralSystem centralSystem = new CentralSystem(driveProvider, directoryProvider, fileProvider);
            DataView dataView = new DataView();
            OptionView optionView = new OptionView();
            Application application = new Application(centralSystem, dataView, optionView);
            
            application.Start();
        }
    }
}