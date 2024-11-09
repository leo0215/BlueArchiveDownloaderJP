using System.Net.Http;
using System.Threading.Tasks;
using BAdownload;
using Newtonsoft.Json.Linq;

class Ver
{
    public static void verMain(string[] args)
    {
        string url = "https://prod-noticeindex.bluearchiveyostar.com/prod/index.json";
        string versionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version");
        string onlineVersionPath = Path.Combine(versionDirectory, "onlineversion.txt");
        string apkVersionPath = Path.Combine(versionDirectory, "apk_version.txt");
        string apkDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "python", "APK");

        try
        {
            string responseBody;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            JObject json = JObject.Parse(responseBody);
            string? latestClientVersion = json["LatestClientVersion"]?.ToString();
            Console.WriteLine("LatestClientVersion: " + latestClientVersion);

            if (!Directory.Exists(versionDirectory))
            {
                Directory.CreateDirectory(versionDirectory);
            }

            File.WriteAllText(onlineVersionPath, latestClientVersion);
            if (File.Exists(apkVersionPath))
            {
                string apkVersion = File.ReadAllText(apkVersionPath);
                if (GlobalData.IsForcedVersion || apkVersion == latestClientVersion)
                {
                    Console.WriteLine("APK version matches the online version. Starting download...");
                    Program.ProgramMain(args).Wait();
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(apkDirectory);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    APK.Main(args);
                }
            }
            else
            {
                APK.Main(args);
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Request error: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Processing error: " + e.Message);
        }
    }
}
