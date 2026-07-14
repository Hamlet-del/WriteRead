using System;
using System.IO;
using System.Text;
using System.Threading;

namespace WriteRead
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = Path.Combine(Path.GetTempPath(), "shared_file.txt");

            string? directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            Console.WriteLine("=== CHOOSE MODE ===");
            Console.WriteLine("1. Writer");
            Console.WriteLine("2. Reader");
            Console.Write("Enter choice (1 or 2): ");
            string? choice = Console.ReadLine()?.Trim().ToLower();

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
            Console.Write("Select flush mode - auto or manual: ");
            string flushChoice = Console.ReadLine()?.Trim().ToLower() ?? "auto";
            bool isAutoFlush = flushChoice == "auto";

            Console.WriteLine($"\nSelected Mode: {(isAutoFlush ? "Auto-Flush" : "Manual Flush")}");
            if (!isAutoFlush)
            {
                Console.WriteLine("Type '/flush' to manually flush your lines.");
            }
            Console.WriteLine("Type 'exit' to quit.\n");

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.AutoFlush = isAutoFlush;

                    while (true)
                    {
                        Console.Write("Enter text: ");
                        string? input = Console.ReadLine();

                        if (string.IsNullOrEmpty(input)) continue;
                        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                        if (!isAutoFlush && input.Equals("/flush", StringComparison.OrdinalIgnoreCase))
                        {
                            writer.Flush();
                            Console.WriteLine("Flushed to file!");
                            continue;
                        }

                        writer.WriteLine(input);
                    }

                    if (!isAutoFlush)
                    {
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing: {ex.Message}");
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

                            if (fs.Length > lastMaxOffset)
                            {
                                fs.Seek(lastMaxOffset, SeekOrigin.Begin);

                                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
                                {
                                    string? line;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        Console.WriteLine(line);
                                    }
                                }
                                lastMaxOffset = fs.Length;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading: {ex.Message}");
                    }
                }
            }
        }
    }
}
