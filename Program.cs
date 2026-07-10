using ConsoleFileManager.Core;
using ConsoleFileManager.Infrastructure;
using ConsoleFileManager.UI;

namespace ConsoleFileManager
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            FileProvider fileProvider = new FileProvider();
            DirectoryProvider directoryProvider = new DirectoryProvider();
            FileSystemService systemService = new FileSystemService(fileProvider, directoryProvider, @"C:\Proga");
            ConsoleApplication console = new ConsoleApplication(systemService);

            await console.Start();
        }
    }
}