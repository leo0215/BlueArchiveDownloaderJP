using System;
using System.IO;
using System.IO.Compression;
using BAdownload;

class APKzip
{
    public static async void zipMain(string[] args)
    {
        if (string.IsNullOrEmpty(GlobalData.XapkFile))
        {
            Console.WriteLine($"Error: APK file not found, the program will be closed");
        }
        //string downloadedApkRelativePath = @"python\APK\com.YostarJP.BlueArchive.apk";
        string extractionRelativePath = @"python\APK\unzip";

        try
        {
            string currentDirectory = Environment.CurrentDirectory;
            string extractionPath = Path.Combine(currentDirectory, extractionRelativePath);
            if (UnpackZip(GlobalData.XapkFile, extractionPath).Result)
            {
                foreach (
                    var apkFile in Directory.GetFiles(
                        extractionPath,
                        "*.apk",
                        SearchOption.TopDirectoryOnly
                    )
                )
                {
                    if (!UnpackZip(apkFile, extractionPath).Result)
                    {
                        Console.WriteLine($"Error unpacking apk file: {apkFile}");
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Error unpacking XAPK file");
                return;
            }

            Console.WriteLine("APK extracted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        await APKver.verMain(args);
    }

    private static Task<bool> UnpackZip(string zipFile, string extractionPath)
    {
        return Task.Run(() =>
        {
            try
            {
                Directory.CreateDirectory(extractionPath);

                using (ZipArchive archive = ZipFile.OpenRead(zipFile))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryFullName = Path.Combine(extractionPath, entry.FullName);
                        string entryDirectory = Path.GetDirectoryName(entryFullName) ?? string.Empty;

                        if (string.IsNullOrEmpty(entryDirectory))
                            continue; // Skip if the entry is for directory

                        if (!Directory.Exists(entryDirectory))
                            Directory.CreateDirectory(entryDirectory);

                        if (File.Exists(entryFullName))
                        {
                            Console.WriteLine($"Skipped: {entryFullName} already exists.");
                            continue; // Skip if the file already exists
                        }

                        entry.ExtractToFile(entryFullName);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        });
    }
}
