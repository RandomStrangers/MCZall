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
    class AutoSaver
    {
        static int _interval;
        const string backupPath = "levels/backups";

        static int count = 1;
        public AutoSaver(int interval)
        {
            _interval = interval * 1000;

            System.Timers.Timer runner = new System.Timers.Timer(_interval);
            runner.Elapsed += delegate
            {
                Exec();

                string allCount = "";
                foreach (Player pl in Player.players) allCount += ", " + pl.name;
                try { Server.s.Log("!PLAYERS ONLINE: " + allCount.Remove(0, 2)); } catch { }

                allCount = "";
                foreach (Level l in Server.levels) allCount += ", " + l.name;
                try { Server.s.Log("!LEVELS ONLINE: " + allCount.Remove(0, 2)); } catch { }
            };
            //Exec();
            runner.Start();
        }

        static void Exec()
        {
            Server.ml.Queue(delegate
            {
                Run();
            });
        }

        static void Run()
        {
            try
            {
                count--;

                Server.levels.ForEach(delegate (Level l)
                {
                    try
                    {
                        if (!l.changed) return;

                        l.Save();
                        if (count == 0)
                        {
                            int backupNumber = l.Backup();

                            if (backupNumber != -1)
                            {
                                foreach (Player p in Player.players)
                                {
                                    if (p.level == l) p.SendMessage("Backup " + backupNumber + " saved.");
                                }
                                Server.s.Log("Backup " + backupNumber + " saved for " + l.name);
                            }
                        }
                    }
                    catch
                    {
                        Server.s.Log("Backup for " + l.name + " has caused an error.");
                    }
                });

                if (count <= 0)
                {
                    count = 15;
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }

            try
            {
                foreach (Player p in Player.players) { p.Save(); }
                Server.s.Log("Saved player database");
            }
            catch (Exception e) { Server.s.Log("Error saving player databases"); Server.s.Log(e.Message); }
        }
    }
}
