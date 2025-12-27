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
using System;

namespace MCZall
{
    public class CmdWhois : Command
    {
        public override string Name { get { return "whois"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdWhois() { }
        public override void Use(Player p, string message)
        {
            Player who;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (who != null && !who.hidden)
            {
                if (who == p)
                {
                    message = Server.DefaultColor + "You are " + who.group.Color + who.prefix + who.group.Name + Server.DefaultColor + ".";
                }
                else
                {
                    message = who.color + who.prefix + who.name + Server.DefaultColor + " is " +
                        who.group.Color + who.group.Name + Server.DefaultColor + " on &b" + who.level.name + Server.DefaultColor + ".";
                }

                if (Server.afkset.Contains(who.name)) message += "-AFK-";

                p.SendMessage(message);
                p.SendMessage(who.color + who.prefix + who.name + Server.DefaultColor + " has :");
                //p.SendMessage("> > &a" + who.money + Server.DefaultColor + " moneys");
                p.SendMessage("> > &cdied &a" + who.overallDeath + Server.DefaultColor + " times");
                p.SendMessage("> > &bmodified &a" + who.overallBlocks + Server.DefaultColor + " blocks, &a" + who.loginBlocks + Server.DefaultColor + " since logging in.");
                string storedTime = Convert.ToDateTime(DateTime.Now.Subtract(who.timeLogged).ToString()).ToString("HH:mm:ss");
                p.SendMessage("> > been logged in for &a" + storedTime);
                p.SendMessage("> > first logged into the server on &a" + who.firstLogin.ToString("yyyy-MM-dd") + " at " + who.firstLogin.ToString("HH:mm:ss"));
                p.SendMessage("> > logged in &a" + who.totalLogins + Server.DefaultColor + " times, &c" + who.totalKicked + Server.DefaultColor + " of which ended in a kick.");
                if (p != null) if (p.group.Permission > LevelPermission.AdvBuilder)
                    {
                        string givenIP;
                        if (Server.bannedIP.Contains(p.ip)) givenIP = "&8" + who.ip + ", which is banned"; else givenIP = who.ip;
                        p.SendMessage("> > the IP of " + givenIP);
                    }
            }
            else { p.SendMessage("\"" + message + "\" is offline! Use /whowas instead."); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/whois [player] - Displays information about someone.");
        }
    }
}