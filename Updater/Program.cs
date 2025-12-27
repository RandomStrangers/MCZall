using System;
using System.Diagnostics;
namespace MCZall.Updater
{
    public class Program
    {
        public const string PrgmName = "MCZall Updater",
            Ver = "1.0.0.1";
        public static string path = System.IO.Directory.GetCurrentDirectory();
        public static void Main(string[] _)
        {
            Console.Title = PrgmName + " " + Ver;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Updating MCZall to latest build from:");
            Console.WriteLine(Updater.BaseURL);
            try
            {
                Updater.PerformUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing update:" + ex);
                Console.ReadKey(false);
                return;
            }
            try
            {
                Process.Start(path + "/MCZallCLI.exe"); //GUI doesn't work on MONO, so use CLI
            }
            catch (Exception e)
            {
                Console.WriteLine("Error performing update:" + e);
                Console.ReadKey(false);
                return;
            }
            Environment.Exit(0);
        }
    }
}