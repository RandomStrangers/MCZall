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
    public class CmdBan : Command
    {
        public override string Name { get { return "ban"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdBan() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            bool stealth = false; bool totalBan = false;
            if (message[0] == '#')
            {
                message = message.Remove(0, 1).Trim();
                stealth = true;
                Server.s.Log("Stealth Ban Atempted");
            }
            else if (message[0] == '@')
            {
                totalBan = true;
                message = message.Remove(0, 1).Trim();
            }

            Player who = Player.Find(message);

            if (who == null)
            {
                if (!Player.ValidName(message)) { p.SendMessage("Invalid name \"" + message + "\"."); return; }
                if (Server.operators.Contains(message)) { p.SendMessage("You can't ban an operator!"); return; }
                if (Server.builders.Contains(message)) { Server.builders.Remove(message); Server.builders.Save("builders.txt"); }
                if (Server.advbuilders.Contains(message)) { Server.advbuilders.Remove(message); Server.advbuilders.Save("advbuilders.txt"); }
                if (Server.superOps.Contains(message)) { p.SendMessage("You can't ban a Super Op!"); return; }
                if (Server.banned.Contains(message)) { p.SendMessage(message + " is already banned."); return; }

                Player.GlobalMessage(message + " &f(offline)" + Server.DefaultColor + " is now &8banned" + Server.DefaultColor + "!");
                Server.banned.Add(message);

            }
            else
            {
                if (!Player.ValidName(who.name)) { p.SendMessage("Invalid name \"" + who.name + "\"."); return; }
                if (Server.operators.Contains(who.name)) { p.SendMessage("You can't ban an operator!"); return; }
                if (Server.builders.Contains(who.name)) { Server.builders.Remove(who.name); Server.builders.Save("builders.txt"); }
                if (Server.advbuilders.Contains(who.name)) { Server.advbuilders.Remove(who.name); Server.advbuilders.Save("advbuilders.txt"); }
                if (Server.superOps.Contains(who.name)) { p.SendMessage("You can't ban a Super Op!"); return; }
                if (Server.banned.Contains(who.name)) { p.SendMessage(message + " is already banned."); return; }

                if (stealth) Player.GlobalMessageOps(who.color + who.name + Server.DefaultColor + " is now STEALTH &8banned" + Server.DefaultColor + "!");
                else Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " is now &8banned" + Server.DefaultColor + "!", false);

                who.group = Group.Find("banned"); who.color = who.group.Color; Player.GlobalDie(who, false);
                Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
                Server.banned.Add(who.name);
            }
            Server.banned.Save("banned.txt", false); IRCBot.Say(message + " was banned by " + p.name);
            Server.s.Log("BANNED: " + message.ToLower());

            if (totalBan == true)
            {
                all.Find("undo").Use(p, message + " 0");
                all.Find("banip").Use(p, message);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/ban <player> - Bans a player without kicking him.");
            p.SendMessage("Add # before name to stealth ban.");
            p.SendMessage("Add @ before name to total ban.");
        }
    }
}