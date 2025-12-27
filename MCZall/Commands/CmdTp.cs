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
    public class CmdTp : Command
    {
        public override string Name { get { return "tp"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdTp() { }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                all.Find("spawn");
                return;
            }
            Player who = Player.Find(message);
            if (who == null || who.hidden) { p.SendMessage("There is no player \"" + message + "\"!"); return; }
            if (p.level != who.level) all.Find("goto").Use(p, who.level.name);
            if (p.level == who.level) unchecked { p.SendPos((byte)-1, who.pos[0], who.pos[1], who.pos[2], who.rot[0], 0); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/tp <player> - Teleports yourself to a player.");
            p.SendMessage("If <player> is blank, /spawn is used.");
        }
    }
}