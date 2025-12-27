using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace MCZall
{
    public class CmdDeleteLvl : Command
    {
        public override string Name { get { return "deletelvl"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdDeleteLvl() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Level foundLevel = Level.Find(message);
            if (foundLevel != null) all.Find("unload").Use(p, foundLevel.name);

            if (foundLevel == Server.mainLevel) { p.SendMessage("Cannot delete the main level."); return; }

            try
            {
                if (!Directory.Exists("levels/deleted")) Directory.CreateDirectory("levels/deleted");

                if (File.Exists("levels/" + message + ".lvl"))
                {
                    if (File.Exists("levels/deleted/" + message + ".lvl"))
                    {
                        int currentNum = 0;
                        while (File.Exists("levels/deleted/" + message + currentNum + ".lvl")) currentNum++;

                        File.Move("levels/" + message + ".lvl", "levels/deleted/" + message + currentNum + ".lvl");
                    }
                    else
                    {
                        File.Move("levels/" + message + ".lvl", "levels/deleted/" + message + ".lvl");
                    }
                    p.SendMessage("Created backup.");

                    File.Delete("levels/level properties/" + message);

                    int TotalTries = 0;
                    MySqlCommand DeleteLvl = Server.mysqlCon.CreateCommand();

                    DeleteLvl.CommandText = "DROP TABLE Block" + message;
                    {
                    retryTag: try { DeleteLvl.ExecuteNonQuery(); } catch (Exception e) { TotalTries++; if (TotalTries < 10) goto retryTag; else Server.ErrorLog(e); }
                    }

                    TotalTries = 0;
                    DeleteLvl.CommandText = "DROP TABLE Portals" + message;
                    {
                    retryTag: try { DeleteLvl.ExecuteNonQuery(); } catch (Exception e) { TotalTries++; if (TotalTries < 10) goto retryTag; else Server.ErrorLog(e); }
                    }

                    TotalTries = 0;
                    DeleteLvl.CommandText = "DROP TABLE Messages" + message;
                    {
                    retryTag: try { DeleteLvl.ExecuteNonQuery(); } catch (Exception e) { TotalTries++; if (TotalTries < 10) goto retryTag; else Server.ErrorLog(e); }
                    }

                    TotalTries = 0;
                    DeleteLvl.CommandText = "DROP TABLE Zone" + message;
                    {
                    retryTag: try { DeleteLvl.ExecuteNonQuery(); } catch (Exception e) { TotalTries++; if (TotalTries < 10) goto retryTag; else Server.ErrorLog(e); }
                    }

                    p.SendMessage("Deleted level.");
                }
                else
                {
                    p.SendMessage("Could not find specified level.");
                }
            }
            catch (Exception e) { p.SendMessage("Error when deleting."); Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/deletelvl [map] - Completely deletes [map] (portals, MBs, everything");
            p.SendMessage("A backup of the map will be placed in the levels/deleted folder");
        }
    }
}