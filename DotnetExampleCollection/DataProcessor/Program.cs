using System;

namespace DataProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("File Procesing Aplication");

            if(args[0] == "--file")
            {
                Console.WriteLine("Process single file");
                processFile( args[1]);
            }
            else if(args[0] == "--dir")
            {
                Console.WriteLine("Process All files in a directory");
                processDirectory(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("Options are not matching");
            }

            Console.ReadLine();
        }

        private static void processDirectory(string directoryName, string fileType)
        {
            var directoryProcessor = new DirectoryProcessor();
            directoryProcessor.Process();
        }

        private static void processFile(string filepath )
        {
            var fileProcessor = new FileProcessor();
            fileProcessor.process(filepath);
        }
    }
}
