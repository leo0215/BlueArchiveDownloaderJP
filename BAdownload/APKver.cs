using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class APKver
{
    public static async Task verMain(string[] args)
    {
        string pythonInterpreterRelativePath = @"python\python.exe";
        string pythonScriptRelativePath = @"python\local_info.py";

        string currentDirectory = Environment.CurrentDirectory;

        string fullPathToPythonInterpreter = Path.GetFullPath(Path.Combine(currentDirectory, pythonInterpreterRelativePath));
        string fullPathToPythonScript = Path.GetFullPath(Path.Combine(currentDirectory, pythonScriptRelativePath));

        Console.WriteLine($"Python Interpreter Path: {fullPathToPythonInterpreter}");
        Console.WriteLine($"Python Script Path: {fullPathToPythonScript}");

        if (!File.Exists(fullPathToPythonInterpreter))
        {
            Console.WriteLine($"Error: Python interpreter not found at {fullPathToPythonInterpreter}");
            return;
        }

        if (!File.Exists(fullPathToPythonScript))
        {
            Console.WriteLine($"Error: Python script not found at {fullPathToPythonScript}");
            return;
        }

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = fullPathToPythonInterpreter,
            Arguments = fullPathToPythonScript,
            WorkingDirectory = Path.GetDirectoryName(fullPathToPythonInterpreter),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            Process? process = Process.Start(start);
            if (process == null)
            {
                Console.WriteLine("Error: Unable to start the process.");
                return;
            }
            using (process)
            {

                using (StreamReader reader = process.StandardOutput)
                {
                    
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using (StreamReader errorReader = process.StandardError)
                {
                    
                    string errors = errorReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errors))
                    {
                        Console.WriteLine($"Errors: {errors}");
                    }
                }

                await process.WaitForExitAsync();
            }

            //because HttpRequestMessege don't have any thread foreground,it will exit if detect function is exit.
            Url.urlMain(args).Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}