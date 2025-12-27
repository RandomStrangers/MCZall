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

namespace MCZall
{
    public class CmdSetRank : Command
    {
        public override string Name { get { return "setrank"; } }
        public override string Shortcut { get { return "rank"; } }
        public override string Type { get { return "mod"; } }
        public CmdSetRank() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length < 2) { Help(p); return; }
            Player who = Player.Find(message.Split(' ')[0]);
            Group newRank = Group.Find(message.Split(' ')[1]);
            string msgGave;

            if (message.Split(' ').Length > 2) msgGave = message.Substring(message.IndexOf(' ', message.IndexOf(' ') + 1)); else msgGave = "Congratulations!";
            if (newRank == null) { p.SendMessage("Could not find specified rank."); return; }

            if (who == null)
            {
                string foundName = message.Split(' ')[0];
                if (Server.banned.Contains(foundName) || newRank.Name == "banned") { p.SendMessage("Cannot change the rank to or from \"banned\"."); return; }

                if (p != null)
                {
                    if (p.group.Permission == LevelPermission.Admin)
                    {
                        if (Server.rankSuper && (newRank.Name == "superop" || Server.superOps.Contains(foundName)))
                        {
                            p.SendMessage("Only the server owner may appoint SuperOPs"); return;
                        }
                    }
                    else
                    {
                        if (Server.operators.Contains(foundName) || Server.superOps.Contains(foundName) || newRank.Permission >= p.group.Permission)
                        {
                            p.SendMessage("Reserved for SuperOPs"); return;
                        }
                    }

                    /*
                    if (p.group.Permission < LevelPermission.Admin) {
                        if ((Server.operators.Contains(foundName) || Server.superOps.Contains(foundName)) || newRank.name == "operator" || newRank.name == "superop") {
                            p.SendMessage("Cannot change the rank of someone equal or higher to yourself."); return; }
                    }*/
                }

                if (Server.superOps.Contains(foundName)) { Server.superOps.Remove(foundName); Server.superOps.Save("uberOps.txt"); }
                if (Server.operators.Contains(foundName)) { Server.operators.Remove(foundName); Server.operators.Save("operators.txt"); }
                if (Server.advbuilders.Contains(foundName)) { Server.advbuilders.Remove(foundName); Server.advbuilders.Save("advbuilders.txt"); }
                if (Server.builders.Contains(foundName)) { Server.builders.Remove(foundName); Server.builders.Save("builders.txt"); }

                switch (newRank.Name)
                {
                    case "builder": Server.builders.Add(foundName); Server.builders.Save("builders.txt"); break;
                    case "advbuilder": Server.advbuilders.Add(foundName); Server.advbuilders.Save("advbuilders.txt"); break;
                    case "operator": Server.operators.Add(foundName); Server.operators.Save("operators.txt"); break;
                    case "superop": Server.superOps.Add(foundName); Server.superOps.Save("uberOps.txt"); break;
                    default: break;
                }

                Player.GlobalMessage(foundName + " &f(offline)" + Server.DefaultColor + "'s rank was set to " + newRank.Color + newRank.Name);
            }
            else if (who == p)
            {
                p.SendMessage("Cannot change your own rank."); return;
            }
            else
            {
                if (Server.banned.Contains(who.name) || newRank.Name == "banned") { p.SendMessage("Cannot change the rank to or from \"banned\"."); return; }

                if (p != null)
                {
                    if (p.group.Permission < LevelPermission.Admin && p != null)
                    {
                        if (Server.operators.Contains(who.name) || Server.superOps.Contains(who.name) || newRank.Name == "operator" || newRank.Name == "superop")
                        {
                            p.SendMessage("Cannot change the rank of someone equal or higher to yourself."); return;
                        }
                    }
                }

                if (Server.superOps.Contains(who.name)) { Server.superOps.Remove(who.name); Server.superOps.Save("uberOps.txt"); }
                if (Server.operators.Contains(who.name)) { Server.operators.Remove(who.name); Server.operators.Save("operators.txt"); }
                if (Server.advbuilders.Contains(who.name)) { Server.advbuilders.Remove(who.name); Server.advbuilders.Save("advbuilders.txt"); }
                if (Server.builders.Contains(who.name)) { Server.builders.Remove(who.name); Server.builders.Save("builders.txt"); }

                switch (newRank.Name)
                {
                    case "builder": Server.builders.Add(who.name); Server.builders.Save("builders.txt"); break;
                    case "advbuilder": Server.advbuilders.Add(who.name); Server.advbuilders.Save("advbuilders.txt"); break;
                    case "operator": Server.operators.Add(who.name); Server.operators.Save("operators.txt"); break;
                    case "superop": Server.superOps.Add(who.name); Server.superOps.Save("uberOps.txt"); break;
                    default: break;
                }

                Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + "'s rank was set to " + newRank.Color + newRank.Name, false);
                Player.GlobalChat(null, "&6" + msgGave, false);
                who.group = newRank;
                who.color = who.group.Color;
                Player.GlobalDie(who, false);
                who.SendMessage("You are now ranked " + newRank.Color + newRank.Name + Server.DefaultColor + ", type /help for your new set of commands.");
                Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/setrank <player> <rank> <yay> - Sets or returns a players rank.");
            p.SendMessage("You may use /rank as a shortcut");
            p.SendMessage("Valid Ranks are: guest, builder, adv, advbuilder, op, operator");
            p.SendMessage("<yay> is a celebratory message");
        }
    }
}
