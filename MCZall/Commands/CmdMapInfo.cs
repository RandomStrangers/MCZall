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
using System.IO;

namespace MCZall
{
    public class CmdMapInfo : Command
    {
        public override string Name { get { return "mapinfo"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdMapInfo() { }
        public override void Use(Player p, string message)
        {
            Level foundLevel;

            if (message == "") { foundLevel = p.level; }
            else foundLevel = Level.Find(message);

            if (foundLevel == null) { p.SendMessage("Could not find specified level."); return; }

            p.SendMessage("&b" + foundLevel.name + Server.DefaultColor + ": Width=" + foundLevel.width.ToString() + " Height=" + foundLevel.depth.ToString() + " Depth=" + foundLevel.height.ToString());

            switch (foundLevel.physics)
            {
                case 0: p.SendMessage("Physics are &cOFF" + Server.DefaultColor + " on &b" + foundLevel.name); break;
                case 1: p.SendMessage("Physics are &aNormal" + Server.DefaultColor + " on &b" + foundLevel.name); break;
                case 2: p.SendMessage("Physics are &aAdvanced" + Server.DefaultColor + " on &b" + foundLevel.name); break;
                case 3: p.SendMessage("Physics are &aHardcore" + Server.DefaultColor + " on &b" + foundLevel.name); break;
                case 4: p.SendMessage("Physics are &aInstant" + Server.DefaultColor + " on &b" + foundLevel.name); break;
            }

            p.SendMessage("Build rank = " + Group.Find(Level.PermissionToName(foundLevel.permissionbuild)).Color + Level.PermissionToName(foundLevel.permissionbuild) + Server.DefaultColor + " : Visit rank = " + Group.Find(Level.PermissionToName(foundLevel.permissionvisit)).Color + Level.PermissionToName(foundLevel.permissionvisit));

            if (Directory.Exists("levels/backups/" + foundLevel.name))
            {
                int latestBackup = Directory.GetDirectories("levels/backups/" + foundLevel.name).Length;
                p.SendMessage("Latest backup: &a" + latestBackup + Server.DefaultColor + " at &a" + Directory.GetCreationTime("levels/backups/" + foundLevel.name + "/" + latestBackup).ToString("yyyy-MM-dd HH:mm:ss")); // + Directory.GetCreationTime("levels/backups/" + latestBackup + "/").ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                p.SendMessage("No backups for this map exist yet.");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/mapinfo <map> - Display details of <map>");
        }
    }
}