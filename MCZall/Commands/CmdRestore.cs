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
    class CmdRestore : Command
    {
        public override string Name { get { return "restore"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }

        public override void Use(Player p, string message)
        {
            //Thread CrossThread;

            if (message != "")
            {
                Server.s.Log("levels/backups/" + p.level.name + "/" + message + "/" + p.level.name + ".lvl");
                if (File.Exists("levels/backups/" + p.level.name + "/" + message + "/" + p.level.name + ".lvl"))
                {
                    try
                    {
                        File.Copy("levels/backups/" + p.level.name + "/" + message + "/" + p.level.name + ".lvl", "levels/" + p.level.name + ".lvl", true);
                        Level temp = new Level("temp", 16, 16, 16, "flat");
                        temp = temp.Load(p.level.name);
                        temp.physThread.Start();
                        if (temp != null)
                        {
                            p.level.spawnx = temp.spawnx;
                            p.level.spawny = temp.spawny;
                            p.level.spawnz = temp.spawnz;

                            p.level.height = temp.height;
                            p.level.width = temp.width;
                            p.level.depth = temp.depth;

                            p.level.blocks = temp.blocks;
                            p.level.SetPhysics(0);
                            p.level.ClearPhysics();

                            all.Find("reveal").Use(p, "all");
                        }
                        else
                        {
                            Server.s.Log("Restore nulled");
                            File.Copy("levels/" + p.level.name + ".lvl.backup", "levels/" + p.level.name + ".lvl", true);
                        }

                    }
                    catch { Server.s.Log("Restore fail"); }
                }
                else { p.SendMessage("Backup " + message + " does not exist."); }
            }
            else
            {
                if (Directory.Exists("levels/backups/" + p.level.name))
                {
                    int backupNumber = Directory.GetDirectories("levels/backups/" + p.level.name).Length;
                    p.SendMessage(p.level.name + " has " + backupNumber.ToString() + " backups .");
                }
                else
                {
                    p.SendMessage(p.level.name + " has no backups yet.");
                }
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/restore <number> - restores a previous backup of the current map");
        }
    }
}
