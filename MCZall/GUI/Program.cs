using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MCZall.Gui
{
    static class Program
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] _)
        {
            bool skip = false;
        remake:
            try
            {
                if (!File.Exists("Viewmode.cfg") || skip)
                {
                    StreamWriter SW = new StreamWriter(File.Create("Viewmode.cfg"));
                    SW.WriteLine("#This file controls how the console window is shown to the server host");
                    SW.WriteLine("#cli:             True or False (Determines whether a CLI interface is used) (Set True if on Mono)");
                    SW.WriteLine("#high-quality:    True or false (Determines whether the GUI interface uses higher quality objects)");
                    SW.WriteLine();
                    SW.WriteLine("cli = false");
                    SW.WriteLine("high-quality = true");
                    SW.Flush();
                    SW.Close();
                    SW.Dispose();
                }

                if (File.ReadAllText("Viewmode.cfg") == "") { skip = true; goto remake; }

                string[] foundView = File.ReadAllLines("Viewmode.cfg");
                if (foundView[0][0] != '#') { skip = true; goto remake; }

                if (foundView[4].Split(' ')[2].ToLower() == "true")
                {
                    Server s = new Server();
                    s.OnLog += Console.WriteLine;
                    s.OnCommand += Console.WriteLine;
                    s.Start();
                    Console.Title = Server.name + " MCZall Version: " + Server.Version;
                    HandleComm(Console.ReadLine());

                    //Application.Run();
                }
                else
                {

                    IntPtr hConsole = GetConsoleWindow();
                    if (IntPtr.Zero != hConsole)
                    {
                        ShowWindow(hConsole, 0);
                    }
                    UpdateCheck();
                    if (foundView[5].Split(' ')[2].ToLower() == "true")
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                    }

                    updateTimer.Elapsed += delegate { UpdateCheck(); }; updateTimer.Start();

                    Application.Run(new Window());
                }
            }
            catch (Exception e) { Server.ErrorLog(e); return; }
        }



        public static void HandleComm(string s)
        {
            string sentCmd, sentMsg = "";

            if (s.IndexOf(' ') != -1)
            {
                sentCmd = s.Split(' ')[0];
                sentMsg = s.Substring(s.IndexOf(' ') + 1);
            }
            else if (s != "")
            {
                sentCmd = s;
            }
            else
            {
                goto talk;
            }

            try
            {
                Command cmd = Command.all.Find(sentCmd);
                if (cmd != null)
                {
                    cmd.Use(null, sentMsg);
                    Console.WriteLine("CONSOLE: USED /" + sentCmd + " " + sentMsg);
                    HandleComm(Console.ReadLine());
                    return;
                }
            }
            catch
            {
                Console.WriteLine("CONSOLE: Failed command.");
                HandleComm(Console.ReadLine());
                return;
            }

        talk: HandleComm("say " + Group.Find("super").Color + "Console: &f" + s);
            HandleComm(Console.ReadLine());
        }

        public static bool CurrentUpdate = false;
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(120 * 60 * 1000);

        public static void UpdateCheck()
        {
            /*
            CurrentUpdate = true;
            Thread updateThread = new Thread(new ThreadStart(delegate {
                WebClient Client = new WebClient();

                StreamWriter SW = new StreamWriter(File.Create("Update.bat"));
                SW.WriteLine("if %1 == main goto main");
                SW.WriteLine("if %1 == kill TASKKILL /PID %2");
                SW.WriteLine("if %1 == delete (if exist MCZall.exe.backup (erase MCZall.exe.backup))");
                SW.WriteLine("if %1 == backup rename MCZall.exe MCZall.exe.backup");
                SW.WriteLine("if %1 == new rename MCZall.new MCZall.exe");
                SW.WriteLine();
                SW.WriteLine("goto end");
                SW.WriteLine(":main");
                SW.WriteLine("Call Update.bat kill %2");
                SW.WriteLine("Call Update.bat delete");
                SW.WriteLine("Call Update.bat backup");
                SW.WriteLine("Call Update.bat new");
                SW.WriteLine("start MCZall.exe");
                SW.WriteLine("exit");
                SW.WriteLine();
                SW.WriteLine(":end");
                SW.Flush();
                SW.Close();
                SW.Dispose();

                if (wait) { if (!Server.checkUpdates) return; Thread.Sleep(10000); }
                try {
                    if (Client.DownloadString("URLGOESHERE") != Server.Version) {
                        if (MessageBox.Show("New version found. Would you like to update?", "Update?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                            Client.DownloadFile("FILEURLGOESHERE", "MCZall.new");

                            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();

                            foreach (Level l in Server.levels) l.Save();
                            foreach (Player pl in Player.players) pl.save();

                            psi.FileName = "Update.bat";
                            psi.Arguments = "main " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                            //psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo = psi;
                            p.Start();
                            p.Dispose();
                        }
                    } else {
                        Server.s.Log("No update for MCZall found! All's good");
                    }
                } catch { Server.s.Log("No web server found to update on."); }
                Client.Dispose();
                CurrentUpdate = false;
            })); updateThread.Start();*/
        }

        public static void ExitProgram(bool AutoRestart)
        {
            Thread exitThread;
            Server.Exit();

            exitThread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string level = null;
                    foreach (Level l in Server.levels)
                    {
                        try
                        {
                            level = level + l.name + "=" + l.physics + Environment.NewLine;
                            l.Save();
                        }
                        catch { }
                    }

                    File.WriteAllText("text/autoload.txt", level);
                    //IRCBot.ShutDown();
                    Logger.Dispose();

                    if (AutoRestart == true) Application.Restart();
                    else Server.process.Kill();
                }
                catch
                {
                    Server.process.Kill();
                }
            })); exitThread.Start();
        }
    }
}
