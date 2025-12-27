using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MCZall
{
    class CmdClones : Command
    {

        public override string Name { get { return "clones"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public override void Use(Player p, string message)
        {
            if (message == "") message = p.name;

            string originalName = message.ToLower();

            Player who = Player.Find(message); int totalCount = 0;
            if (who == null)
            {
                p.SendMessage("Could not find player. Searching Player DB.");

                DataTable FindIP = new DataTable("FindIP");
            retryTag1: try
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT IP FROM Players WHERE Name='" + message + "'", Server.mysqlCon))
                    { da.Fill(FindIP); }
                }
                catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag1; else { Server.ErrorLog(e); return; } }

                if (FindIP.Rows.Count == 0) { p.SendMessage("Could not find any player by the name entered."); FindIP.Dispose(); return; }

                message = FindIP.Rows[0]["IP"].ToString();
                FindIP.Dispose();
            }
            else
            {
                message = who.ip;
            }

            DataTable Clones = new DataTable("Clones");
            totalCount = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT Name FROM Players WHERE IP='" + message + "'", Server.mysqlCon))
                { da.Fill(Clones); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else { Server.ErrorLog(e); return; } }

            if (Clones.Rows.Count == 0) { p.SendMessage("Could not find any record of the player entered."); return; }

            List<string> foundPeople = new List<string>
            {
                originalName
            };

            for (int i = 0; i < Clones.Rows.Count; ++i)
            {
                if (!foundPeople.Contains(Clones.Rows[i]["Name"].ToString().ToLower()))
                    foundPeople.Add(Clones.Rows[i]["Name"].ToString().ToLower());
            }

            Clones.Dispose();
            if (foundPeople.Count <= 1) { p.SendMessage(originalName + " has no clones."); return; }

            p.SendMessage("These people have the same IP address:");
            p.SendMessage(string.Join(", ", foundPeople.ToArray()));
        }

        public override void Help(Player p)
        {
            p.SendMessage("/clones <name> - Finds everyone with the same IP has <name>");
        }
    }
}
