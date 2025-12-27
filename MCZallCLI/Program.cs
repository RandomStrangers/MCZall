using System;
using System.IO;
using System.Diagnostics;

namespace MCZall.CLI
{
    public static class Program
    {
        public static void Main(string[] _)
        {
            try
            {
                Server s = new Server();
                s.OnLog += Console.WriteLine;
                s.OnCommand += Console.WriteLine;
                s.Start();
                Console.Title = Server.name + " MCZall Version: " + Server.Version;
                HandleComm(Console.ReadLine());
            }
            catch (Exception e) 
            { 
                Server.ErrorLog(e);
                return;
            }
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
                HandleComm("say " + Group.Find("super").Color + "Console: &f" + s);
                HandleComm(Console.ReadLine());
                return;
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
        }
        public static void ExitProgram(bool AutoRestart)
        {
            Server.Exit();
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
                    catch 
                    { 
                    }
                }
                File.WriteAllText("text/autoload.txt", level);
                Logger.Dispose();
                string prgm = Server.process.MainModule.FileName;
                if (AutoRestart)
                {
                    Process.Start(prgm);
                    Server.process.Kill();
                }
                else
                {
                    Server.process.Kill();
                }
            }
            catch
            {
                Server.process.Kill();
            }
        }
    }
}
