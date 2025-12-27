using System.Collections.Generic;
using System.IO;

namespace MCZall
{
    public class CmdUnloaded : Command
    {
        public override string Name { get { return "unloaded"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdUnloaded() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            List<string> levels = new List<string>(Server.levels.Count);

            message = "";
            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] fi = di.GetFiles("*.lvl");
            int iLoop = 0;
            p.SendMessage("Unloaded levels: ");
            foreach (Level l in Server.levels) { levels.Add(l.name.ToLower()); }
            foreach (FileInfo file in fi)
            {
                if (!levels.Contains(file.Name.Replace(".lvl", "").ToLower()))
                {
                    if (message == "")
                    {
                        iLoop += 1;
                        message += file.Name.Replace(".lvl", "");
                    }
                    else
                    {
                        iLoop += 1;
                        message += ", " + file.Name.Replace(".lvl", "");
                    }
                }
                if (iLoop == 4) { p.SendMessage(">&4" + message); iLoop = 0; message = ""; }
            }
            if (message != "") { p.SendMessage(">&4" + message); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/unloaded - Lists all unloaded levels.");
        }
    }
}