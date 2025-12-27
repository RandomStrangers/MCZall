using MySql.Data.MySqlClient;
using System;

namespace MCZall
{
    public class CmdClearBlockChanges : Command
    {
        public override string Name { get { return "clearblockchanges"; } }
        public override string Shortcut { get { return "cbc"; } }
        public override string Type { get { return "mod"; } }

        public CmdClearBlockChanges() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            MySqlCommand TruncateMe = Server.mysqlCon.CreateCommand();
            TruncateMe.CommandText = "TRUNCATE TABLE Block" + p.level.name;
        retryTag1: try { TruncateMe.ExecuteNonQuery(); } catch (Exception e) { Server.ErrorLog(e); goto retryTag1; }

            p.SendMessage("Cleared &cALL" + Server.DefaultColor + " recorded block changes in: &d" + p.level.name);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/clearblockchanges - Clears the block changes stored in /about for the current map.");
            p.SendMessage("&cUSE WITH CAUTION");
        }
    }
}