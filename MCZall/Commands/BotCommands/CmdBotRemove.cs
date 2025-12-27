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
    public class CmdBotRemove : Command
    {
        public override string Name { get { return "botremove"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdBotRemove() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (message.ToLower() == "all")
            {
                bool oneRemove = false;
                foreach (PlayerBot Pb in PlayerBot.playerbots)
                {
                    if (Pb.level == p.level)
                    {
                        Pb.RemoveBot();
                        p.SendMessage("Removed " + Pb.name);
                        oneRemove = true;
                    }
                }
                if (!oneRemove) p.SendMessage("No bots were found on the current level");
                return;
            }

            PlayerBot who = PlayerBot.Find(message);
            if (who == null) { p.SendMessage("There is no bot " + who + "!"); return; }
            if (p.level != who.level) { p.SendMessage(who.name + " is in a different level."); return; }
            who.RemoveBot();
            p.SendMessage("Removed bot.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/botremove <name> <all> - Remove a bot on the same level as you");
            p.SendMessage("If All is used, all bots on the current level are removed");
        }
    }
}