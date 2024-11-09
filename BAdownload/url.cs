using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Url
{
    public static async Task urlMain(string[] args)
    {
        string urlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "python", "APK", "url.txt");
        try
        {
            string urlContent = File.ReadAllText(urlFilePath);
            Console.WriteLine("URL Content:");
            Console.WriteLine(urlContent);

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                    using HttpResponseMessage response = await client.GetAsync(urlContent);
                    Console.WriteLine($"Response status code: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(jsonContent);
                        JToken? overrideGroups = json.SelectToken("ConnectionGroups[0].OverrideConnectionGroups");

                        if (overrideGroups != null && overrideGroups.HasValues)
                        {
                            bool foundSecondRoot = false;
                            foreach (var group in overrideGroups)
                            {
                                string addressablesCatalogUrlRoot = group.Value<string>("AddressablesCatalogUrlRoot") ?? string.Empty;
                                if (!string.IsNullOrEmpty(addressablesCatalogUrlRoot))
                                {
                                    if (foundSecondRoot)
                                    {
                                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "python", "APK", "AddressablesCatalogUrlRoot.txt");
                                        await File.WriteAllTextAsync(filePath, addressablesCatalogUrlRoot);
                                        Console.WriteLine("AddressablesCatalogUrlRoot: " + addressablesCatalogUrlRoot);
                                        break;
                                    }
                                    else
                                    {
                                        foundSecondRoot = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("OverrideConnectionGroups not found in JSON.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: Failed to get JSON data. Status code: {response.StatusCode}");
                    }

                }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Error: File not found: " + e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("Error: Unauthorized access: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        try
        {
            Ver.verMain(args);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error calling verMain: " + e.Message);
        }
    }
}