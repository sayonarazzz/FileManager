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
            TextSerializer serializer = new TextSerializer();
            DirectoryProvider  directoryProvider = new DirectoryProvider();
            FileService<string> fileService = new FileService<string>(fileProvider, serializer);
            ConsoleApplication console = new ConsoleApplication(fileService, directoryProvider);

            await console.Start();
        }
    }
}