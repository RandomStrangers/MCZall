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
    public class CmdPhysics : Command
    {
        public override string Name { get { return "physics"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdPhysics() { }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                foreach (Level l in Server.levels)
                {
                    if (l.physics > 0)
                        p.SendMessage("&5" + l.name + Server.DefaultColor + " has physics at &b" + l.physics + Server.DefaultColor + ". &cChecks: " + l.lastCheck + "; Updates: " + l.lastUpdate);
                }
                return;
            }
            try
            {
                int temp = 0; Level level = null;
                if (message.Split(' ').Length == 1)
                {
                    temp = int.Parse(message);
                    level = p.level;
                }
                else
                {
                    temp = Convert.ToInt16(message.Split(' ')[1]);
                    string nameStore = message.Split(' ')[0];
                    level = Level.Find(nameStore);
                }
                if (temp >= 0 && temp <= 4)
                {
                    level.SetPhysics(temp);
                    switch (temp)
                    {
                        case 0:
                            level.ClearPhysics();
                            Player.GlobalMessage("Physics are now &cOFF" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now OFF on " + level.name + ".");
                            IRCBot.Say("Physics are now OFF on " + level.name + ".");
                            break;

                        case 1:
                            Player.GlobalMessage("Physics are now &aNormal" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now ON on " + level.name + ".");
                            IRCBot.Say("Physics are now ON on " + level.name + ".");
                            break;

                        case 2:
                            Player.GlobalMessage("Physics are now &aAdvanced" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now ADVANCED on " + level.name + ".");
                            IRCBot.Say("Physics are now ADVANCED on " + level.name + ".");
                            break;

                        case 3:
                            Player.GlobalMessage("Physics are now &aHardcore" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now HARDCORE on " + level.name + ".");
                            IRCBot.Say("Physics are now HARDCORE on " + level.name + ".");
                            break;

                        case 4:
                            Player.GlobalMessage("Physics are now &aInstant" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now INSTANT on " + level.name + ".");
                            IRCBot.Say("Physics are now INSTANT on " + level.name + ".");
                            break;
                    }
                }
                else
                {
                    p?.SendMessage("Not a valid setting");
                }
            }
            catch
            {
                p?.SendMessage("INVALID INPUT");
            }

        }

        public override void Help(Player p)
        {
            p.SendMessage("/physics [map] <0/1/2/3/4> - Set the [map]'s physics, 0-Off 1-On 2-Advanced 3-Hardcore 4-Instant");
            p.SendMessage("If [map] is blank, uses Current level");
        }
    }
}