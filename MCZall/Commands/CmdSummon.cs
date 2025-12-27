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
    public class CmdSummon : Command
    {
        public override string Name { get { return "summon"; } }
        public override string Shortcut { get { return "s"; } }
        public override string Type { get { return "other"; } }
        public CmdSummon() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message);
            if (who == null || who.hidden) { p.SendMessage("There is no player \"" + message + "\"!"); return; }
            if (p.level != who.level) { p.SendMessage(who.name + " is in a different level."); return; }
            unchecked { who.SendPos((byte)-1, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0); }
            who.SendMessage("You were summoned by " + p.color + p.name + Server.DefaultColor + ".");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/summon <player> - Summons a player to your position.");
        }
    }
}