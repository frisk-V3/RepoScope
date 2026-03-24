using System;
using System.Net.Http;
using System.Text.Json;
using System.IO;

class Program
{
    static async System.Threading.Tasks.Task Main()
    {
        Console.WriteLine("=== RepoScope (Single-file GitHub Repo Inspector) ===\n");

        Console.Write("Owner: ");
        string owner = Console.ReadLine();

        Console.Write("Repository: ");
        string repo = Console.ReadLine();

        string apiUrl = $"https://api.github.com/repos/{owner}/{repo}";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("RepoScope-App");

        try
        {
            string json = await client.GetStringAsync(apiUrl);
            var doc = JsonDocument.Parse(json).RootElement;

            Console.WriteLine("\n=== Repository Info ===");
            Console.WriteLine($"Name: {doc.GetProperty("full_name")}");
            Console.WriteLine($"Stars: {doc.GetProperty("stargazers_count")}");
            Console.WriteLine($"Forks: {doc.GetProperty("forks_count")}");
            Console.WriteLine($"Description: {doc.GetProperty("description")}");
            Console.WriteLine($"Language: {doc.GetProperty("language")}");
            Console.WriteLine($"Size: {doc.GetProperty("size")} KB");
            Console.WriteLine($"URL: {doc.GetProperty("html_url")}");
            Console.WriteLine("========================\n");

            Console.Write("Download ZIP? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                string zipUrl = doc.GetProperty("html_url").GetString() + "/archive/refs/heads/main.zip";
                string fileName = $"{repo}-main.zip";

                Console.WriteLine("Downloading: " + zipUrl);

                var bytes = await client.GetByteArrayAsync(zipUrl);
                File.WriteAllBytes(fileName, bytes);

                Console.WriteLine("Saved as: " + fileName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
