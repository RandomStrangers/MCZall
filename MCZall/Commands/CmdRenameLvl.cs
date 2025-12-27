using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace MCZall
{
    public class CmdRenameLvl : Command
    {
        public override string Name { get { return "renamelvl"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdRenameLvl() { }
        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            Level foundLevel = Level.Find(message.Split(' ')[0]);
            string newName = message.Split(' ')[1];

            if (File.Exists("levels/" + newName)) { p.SendMessage("Level already exists."); return; }
            if (foundLevel == Server.mainLevel) { p.SendMessage("Cannot rename the main level."); return; }
            if (foundLevel != null) all.Find("unload").Use(p, foundLevel.name);

            try
            {
                File.Move("levels/" + foundLevel.name + ".lvl", "levels/" + newName + ".lvl");
                File.Move("levels/level properties/" + foundLevel.name, "levels/level properties/" + newName);

                MySqlCommand RenameTab = new MySqlCommand("RENAME TABLE Block" + foundLevel.name.ToLower() + " TO Block" + newName.ToLower() +
                    ", Portals" + foundLevel.name.ToLower() + " TO Portals" + newName.ToLower() +
                    ", Messages" + foundLevel.name.ToLower() + " TO Messages" + newName.ToLower() +
                    ", Zone" + foundLevel.name.ToLower() + " TO Zone" + newName.ToLower(), p.playerCon);
                RenameTab.ExecuteNonQuery();

                Player.GlobalMessage("Renamed " + foundLevel.name + " to " + newName);
            }
            catch (Exception e) { p.SendMessage("Error when renaming."); Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/renamelvl <level> <new name> - Renames <level> to <new name>");
            p.SendMessage("Portals going to <level> will be lost");
        }
    }
}