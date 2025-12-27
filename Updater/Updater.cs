using System;
using System.IO;
using System.Net;
namespace MCZall.Updater
{
    public static class Updater
    {
        public const string BaseURL = "https://github.com/RandomStrangers/MCZall/raw/master/Uploads/",
            cli = BaseURL + "MCZallCLI.exe",
            exe = BaseURL + "MCZall.exe";
        public static void PerformUpdate()
        {
            try
            {
                try
                {
                    TryDelete("MCZall.update", "MCZallCLI.update",
                        "prev_MCZall.exe", "prev_MCZallCLI.exe");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.ReadKey(false);
                    return;
                }
                try
                {
                    WebClient client = new WebClient();
                    File.Move("MCZall.exe", "prev_MCZall.exe");
                    File.Move("MCZallCLI.exe", "prev_MCZallCLI.exe");
                    client.DownloadFile(cli, "MCZallCLI.update");
                    client.DownloadFile(exe, "MCZall.update");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.ReadKey(false);
                    return;
                }
                File.Move("MCZall.update", "MCZall.exe");
                File.Move("MCZallCLI.update", "MCZallCLI.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.ReadKey(false);
                return;
            }
        }
        public static void TryDelete(params string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    File.Delete(path);
                }
                catch (FileNotFoundException)
                {
                }
            }
        }
    }
}