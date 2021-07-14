using System;
using System.IO;

namespace DataProcessor
{
    public class FileProcessor
    {
        const string backupdirectory = "backup";
        const string processdirectory = "processed";
        const string completeddirectory = "completed";
       
        public FileProcessor()
        {
        }

        public  void process(string filePath)
        {
            Console.WriteLine($"Input File Path is {filePath}");

            //chek file exsist or not
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error : File not exsist in Input Path");
                return;
            }

            var rootPath = (new DirectoryInfo(filePath)).Parent.Parent.FullName;
            Console.WriteLine($"Root Path is {rootPath}");

            //backup the file
            var backupdirectoryPath = Path.Combine(rootPath, backupdirectory);

            Console.WriteLine("Creating Backup Directory");
            Directory.CreateDirectory(backupdirectoryPath);
            Console.WriteLine("Creating Precsssed Directory");
            Directory.CreateDirectory(Path.Combine(rootPath, processdirectory));
            Console.WriteLine("Creating completed Directory");
            Directory.CreateDirectory(Path.Combine(rootPath, completeddirectory));

            var inputfilename = Path.GetFileName(filePath);
            var backfilePath = Path.Combine(backupdirectoryPath, inputfilename);

            Console.WriteLine("Backing up Input file.....");

            File.Copy(filePath, backfilePath, true);

            if (!File.Exists(backfilePath))
            {
                Console.WriteLine("Error : Backup process failed");
                return;
            }

            Console.WriteLine("Start Processing the file");
            var processfilePath = Path.Combine(rootPath, processdirectory, inputfilename);
            var completedPath = Path.Combine(rootPath, completeddirectory, inputfilename);
            startProcess(processfilePath);

            File.Move(backfilePath, processfilePath, true);

            Console.WriteLine("Move Prosesed file to Completed");

            File.Move(processfilePath, completedPath,true);

            Console.WriteLine("Delete Processed directory");
            Directory.Delete(Path.Combine(rootPath, processdirectory), true);
        }

        private  void startProcess(string path)
        {
            if (File.Exists(path))
            {
                Console.WriteLine("File alredy Processed");
                return;
            }

            Console.WriteLine($"Processing the file {path}");
        }
    }
}