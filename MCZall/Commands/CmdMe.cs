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
    public class CmdMe : Command
    {
        public override string Name { get { return "me"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdMe() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { p.SendMessage("You"); return; }
            else
            {
                if (Server.worldChat)
                {
                    Player.GlobalChat(p, p.color + "*" + p.name + " " + message, false);
                }
                else
                {
                    Player.GlobalChatLevel(p, p.color + "*" + p.name + " " + message, false);
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("What do you need help with, m'boy?! Are you stuck down a well?!");
        }
    }
}