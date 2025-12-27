/*
	Copyright 2010 MCZall Team Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace MCZall
{
    public class CmdWhowas : Command
    {
        public override string Name { get { return "whowas"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdWhowas() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player pl = Player.Find(message); if (pl != null && !pl.hidden)
            { p.SendChat(p, pl.color + pl.name + Server.DefaultColor + " is online, use /whois instead."); return; }

            if (message.IndexOf("'") != -1) { p.SendMessage("Cannot parse request."); return; }

            DataTable playerDb = new DataTable("playerDb"); int totalCount = 0;

        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Players WHERE Name='" + message + "'", Server.mysqlCon))
                { da.Fill(playerDb); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else { Server.ErrorLog(e); return; } }

            string FoundRank;

            if (Server.banned.Contains(message.ToLower())) FoundRank = "Banned";
            else if (Server.builders.Contains(message.ToLower())) FoundRank = "Builder";
            else if (Server.advbuilders.Contains(message.ToLower())) FoundRank = "AdvBuilder";
            else if (Server.operators.Contains(message.ToLower())) FoundRank = "Operator";
            else if (Server.superOps.Contains(message.ToLower())) FoundRank = "SuperOP";
            else FoundRank = "Guest";

            if (playerDb.Rows.Count == 0) { p.SendMessage(Group.Find(FoundRank).Color + message + Server.DefaultColor + " has the rank of " + Group.Find(FoundRank).Color + FoundRank); return; }

            p.SendMessage(Group.Find(FoundRank).Color + playerDb.Rows[0]["Title"] + message + Server.DefaultColor + " has :");
            p.SendMessage("> > the rank of \"" + Group.Find(FoundRank).Color + FoundRank + Server.DefaultColor + "\".");
            //p.SendMessage("> > &a" + playerDb.Rows[0]["Money"] + Server.DefaultColor + " moneys");
            p.SendMessage("> > &cdied &a" + playerDb.Rows[0]["TotalDeaths"] + Server.DefaultColor + " times");
            p.SendMessage("> > &bmodified &a" + playerDb.Rows[0]["totalBlocks"] + Server.DefaultColor + " blocks.");
            p.SendMessage("> > was last seen on &a" + playerDb.Rows[0]["LastLogin"]);
            p.SendMessage("> > first logged into the server on &a" + playerDb.Rows[0]["FirstLogin"]);
            p.SendMessage("> > logged in &a" + playerDb.Rows[0]["totalLogin"] + Server.DefaultColor + " times, &c" + playerDb.Rows[0]["totalKicked"] + Server.DefaultColor + " of which ended in a kick.");
            if (p != null) if (p.group.Permission > LevelPermission.AdvBuilder || p == null)
                {
                    if (Server.bannedIP.Contains(playerDb.Rows[0]["IP"].ToString()))
                        playerDb.Rows[0]["IP"] = "&8" + playerDb.Rows[0]["IP"] + ", which is banned";
                    p.SendMessage("> > the IP of " + playerDb.Rows[0]["IP"]);
                }
            playerDb.Dispose();
        }
        public override void Help(Player p)
        {
            p.SendMessage("/whowas <name> - Displays information about someone who left.");
        }
    }
}