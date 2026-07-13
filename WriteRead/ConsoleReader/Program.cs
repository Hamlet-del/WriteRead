using System;
using System.IO;
using System.Threading;

namespace WriteRead
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\temp\shared_file.txt";

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            Console.WriteLine("=== CHOOSE MODE ===");
            Console.WriteLine("1. Writer");
            Console.WriteLine("2. Reader");
            Console.Write("Enter choice (1 or 2): ");
            string choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                RunWriter(filePath);
            }
            else if (choice == "2")
            {
                RunReader(filePath);
            }
            else
            {
                Console.WriteLine("Invalid choice. Exiting...");
            }
        }

        static void RunWriter(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing file: {ex.Message}");
            }

            Console.Clear();
            Console.WriteLine("=== CONSOLE WRITER ===");
            Console.WriteLine("Type text and press Enter to write. Type 'exit' to quit.\n");

            while (true)
            {
                Console.Write("Enter text: ");
                string input = Console.ReadLine();

                if (input?.ToLower() == "exit") break;

                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(input);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing: {ex.Message}");
                }
            }
        }

        static void RunReader(string filePath)
        {
            long lastMaxOffset = 0;

            Console.Clear();
            Console.WriteLine("=== CONSOLE READER ===");
            Console.WriteLine("Waiting for text...\n");

            while (true)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            if (fs.Length < lastMaxOffset)
                            {
                                lastMaxOffset = 0;
                                Console.Clear();
                                Console.WriteLine("=== CONSOLE READER ===");
                                Console.WriteLine("Waiting for text...\n");
                            }

                            fs.Seek(lastMaxOffset, SeekOrigin.Begin);

                            using (StreamReader reader = new StreamReader(fs))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    Console.WriteLine(line);
                                }

                                lastMaxOffset = fs.Position;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}