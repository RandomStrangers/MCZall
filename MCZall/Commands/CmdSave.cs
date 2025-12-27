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
    public class CmdSave : Command
    {
        public override string Name { get { return "save"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdSave() { }
        public override void Use(Player p, string message)
        {
            if (p != null)
            {
                if (message != "") { Help(p); return; }
                p.level.Save(true);
                p.SendMessage("Level \"" + p.level.name + "\" saved.");

                int backupNumber = p.level.Backup(true);
                if (backupNumber != -1)
                {
                    Player.GlobalChatLevel(p, "Backup " + backupNumber + " saved.", false);
                    Server.s.Log("Backup " + backupNumber + " saved for " + p.level.name);
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/save - Saves the level, not an actual backup.");
        }
    }
}