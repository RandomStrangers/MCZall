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
using System.Collections.Generic;

namespace MCZall
{
    public class CmdLevels : Command
    {
        public override string Name { get { return "levels"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdLevels() { }
        public override void Use(Player p, string message)
        { // TODO
            if (message != "") { Help(p); return; }
            List<string> levels = new List<string>(Server.levels.Count);
            message = Server.mainLevel.name + " &b[" + Server.mainLevel.physics + "]";
            string message2 = "";
            levels.Add(Server.mainLevel.name.ToLower());
            bool Once = false;
            Server.levels.ForEach(delegate (Level level)
            {
                if (level != Server.mainLevel)
                {
                    if (level.permissionvisit <= p.group.Permission)
                    {
                        message += ", &2" + level.name + " &b[" + level.physics + "]";
                        levels.Add(level.name.ToLower());
                    }
                    else
                    {
                        if (!Once)
                        {
                            Once = true;
                            message2 += "&2" + level.name + " &b[" + level.physics + "]";
                        }
                        else
                        {
                            message2 += ", &2" + level.name + " &b[" + level.physics + "]";
                        }
                    }
                }
            });
            p.SendMessage("Loaded: &2" + message);
            p.SendMessage("Can't Goto: &c" + message2);
            p.SendMessage("Use &4/unloaded for unloaded levels.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/levels - Lists all loaded levels and their physics levels.");
        }
    }
}