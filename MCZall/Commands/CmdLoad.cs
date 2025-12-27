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
using System.IO;

namespace MCZall
{
    public class CmdLoad : Command
    {
        public override string Name { get { return "load"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdLoad() { }
        public override void Use(Player p, string message)
        {
            //Server.ml.Queue(delegate {
            try
            {
                if (message == "") { Help(p); return; }
                if (message.Split(' ').Length > 2) { Help(p); return; }
                int pos = message.IndexOf(' ');
                string phys = "0";
                if (pos != -1)
                {
                    phys = message.Substring(pos + 1);
                    message = message.Substring(0, pos).ToLower();
                }
                else
                {
                    message = message.ToLower();
                }

                foreach (Level l in Server.levels)
                {
                    if (l.name == message) { p?.SendMessage(message + " is already loaded!"); return; }
                }

                if (Server.levels.Count == Server.levels.Capacity)
                {
                    if (Server.levels.Capacity == 1)
                    {
                        p?.SendMessage("You can't load any levels!");
                    }
                    else
                    {
                        all.Find("unload").Use(p, "empty");
                        if (Server.levels.Capacity == 1)
                        {
                            p?.SendMessage("No maps are empty to unload. Cannot load map.");
                            return;
                        }
                    }
                }

                if (!File.Exists("levels/" + message + ".lvl"))
                {
                    p?.SendMessage("Level \"" + message + "\" doesn't exist!"); return;
                }

                Level level = new Level("temp", 16, 16, 16, "flat");
                level = level.Load(message);

                if (level == null)
                {
                    if (File.Exists("levels/" + message + ".lvl.backup"))
                    {
                        Server.s.Log("Attempting to load backup.");
                        File.Copy("levels/" + message + ".lvl.backup", "levels/" + message + ".lvl", true);
                        level = level.Load(message);
                        if (level == null)
                        {
                            if (p != null) if (!p.disconnected) p.SendMessage("Backup of " + message + " failed.");
                            return;
                        }
                    }
                    else
                    {
                        if (p != null) if (!p.disconnected) p.SendMessage("Backup of " + message + " does not exist.");
                        return;
                    }

                }
                lock (Server.levels)
                {
                    Server.levels.Add(level);
                }
                level.physThread.Start();
                Player.GlobalMessage("Level \"" + level.name + "\" loaded.");
                try
                {
                    int temp = int.Parse(phys);
                    if (temp >= 0 && temp <= 4)
                    {
                        level.SetPhysics(temp);
                    }
                }
                catch { if (p != null) if (!p.disconnected) p.SendMessage("Physics variable invalid"); }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception e) { Player.GlobalMessage("An error occured with /load"); Server.ErrorLog(e); }
            //});
        }
        public override void Help(Player p)
        {
            p.SendMessage("/load <level> - Loads a level.");
        }
    }
}