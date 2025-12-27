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
    public class CmdUnload : Command
    {
        public override string Name { get { return "unload"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdUnload() { }
        public override void Use(Player p, string message)
        {
            if (message.ToLower() == "empty")
            {
                bool Empty = true;

                foreach (Level l in Server.levels)
                {
                    Empty = true;
                    Player.players.ForEach(delegate (Player pl)
                    {
                        if (pl.level == l) Empty = false;
                    });
                    if (Empty == true && l.unload)
                    {
                        l.Save();
                        l.physThread.Abort();
                        Server.levels.Remove(l);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        Player.GlobalMessageOps("&3" + l.name + Server.DefaultColor + " was unloaded.");
                        return;
                    }
                }
                p.SendMessage("No levels were empty.");
                return;
            }

            Level level = Level.Find(message);

            if (level != null)
            {
                if (level == Server.mainLevel) { p.SendMessage("You can't unload the main level."); return; }

                Player.players.ForEach(delegate (Player pl)
                {
                    if (pl.level == level)
                        if (p != null)
                        {
                            if (pl != p) all.Find("goto").Use(pl, "main");
                        }
                        else all.Find("goto").Use(pl, "main");
                });

                if (p != null) if (p.level == level) { all.Find("goto").Use(p, "main"); }
                level.Save();
                level.physThread.Abort();
                Server.levels.Remove(level);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Player.GlobalMessageOps("&3" + level.name + Server.DefaultColor + " was unloaded.");
                return;
            }

            p.SendMessage("There is no level \"" + message + "\" loaded.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/unload [level] - Unloads a level.");
            p.SendMessage("/unload empty - Unloads an empty level.");
        }
    }
}