using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestGeneratorDll;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your list of file to generate tests for them separating them by spaces");
            List<string> Files = Console.ReadLine().Split(' ').ToList();
            List<string> FilesPath = new List<string>();
            foreach (string File in Files)
            {
                FilesPath.Add(Path.GetFullPath(File));
            }
            Console.WriteLine("Enter test folder");
            string Folder = Path.GetFullPath(Console.ReadLine());
            Console.WriteLine("Enter max amount of threads for generating tests");
            int Threads = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter max amount of threads for reading files");
            int FilesToRead = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter max amount of threads for writing files");
            int FilesToWrite = int.Parse(Console.ReadLine());
            TestGenerator generator = new TestGenerator();
            Pipeline pipeline = new Pipeline(generator,FilesPath, Folder, Threads, FilesToRead, FilesToWrite);
            try
            {
                pipeline.GenerateAsync().Wait();
                Console.WriteLine("Completed.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
