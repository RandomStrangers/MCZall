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
    public class CmdUnban : Command
    {
        public override string Name { get { return "unban"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdUnban() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message);

            if (who == null)
            {
                if (!Server.banned.Contains(message)) { p.SendMessage("Player is not banned."); return; }
                Player.GlobalMessage(message + " &8(banned)" + Server.DefaultColor + " is now " + Group.standard.Color + Group.standard.Name + Server.DefaultColor + "!");
                Server.banned.Remove(message);
            }
            else
            {
                if (!Server.banned.Contains(who.name)) { p.SendMessage("Player is not banned."); return; }
                Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " is now " + Group.standard.Color + Group.standard.Name + Server.DefaultColor + "!", false);
                who.group = Group.standard; who.color = who.group.Color; Player.GlobalDie(who, false);
                Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
                Server.banned.Remove(who.name);
            }

            Server.banned.Save("banned.txt", false);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/unban <player> - Unbans a player.");
        }
    }
}