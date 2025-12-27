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
    public class CmdEmote : Command
    {
        public override string Name { get { return "emote"; } }
        public override string Shortcut { get { return "<3"; } }
        public override string Type { get { return "other"; } }
        public CmdEmote() { }
        public override void Use(Player p, string message)
        {
            p.parseSmiley = !p.parseSmiley;
            p.smileySaved = false;

            if (p.parseSmiley) p.SendMessage("Emote parsing is enabled.");
            else p.SendMessage("Emote parsing is disabled.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/emote - Enables or disables emoticon parsing");
        }
    }
}