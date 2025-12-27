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
    public class CmdKick : Command
    {
        public override string Name { get { return "kick"; } }
        public override string Shortcut { get { return "k"; } }
        public override string Type { get { return "mod"; } }
        public CmdKick() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { p.SendMessage("Could not find player specified."); return; }
            if (message.Split(' ').Length > 1)
                message = message.Substring(message.IndexOf(' ') + 1);
            else
                if (p == null) message = "You were kicked by Nobody!"; else message = "You were kicked by " + p.name + "!";

            if (p != null)
                if (who == p) { p.SendMessage("You cannot kick yourself!"); return; }
                else if (who.group.Permission >= p.group.Permission && p != null) { Player.GlobalChat(p, p.color + p.name + Server.DefaultColor + " tried to kick " + who.color + who.name + " but failed.", false); return; }

            who.Kick(message);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/kick <player> [message] - Kicks a player.");
        }
    }
}