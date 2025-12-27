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
    class CmdNewLvl : Command
    {
        public override string Name { get { return "newlvl"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdNewLvl() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            string[] parameters = message.Split(' '); // Grab the parameters from the player's message
            if (parameters.Length == 5) // make sure there are 5 params
            {
                switch (parameters[4])
                {
                    case "flat":
                    case "pixel":
                    case "island":
                    case "mountains":
                    case "ocean":
                    case "forest":
                    case "desert":
                        break;

                    default:
                        p.SendMessage("Valid types: island, mountains, forest, ocean, flat, pixel, desert"); return;
                }

                string name = parameters[0];
                ushort x, y, z;
                try
                {
                    x = Convert.ToUInt16(parameters[1]);
                    y = Convert.ToUInt16(parameters[2]);
                    z = Convert.ToUInt16(parameters[3]);
                }
                catch { p.SendMessage("Invalid dimensions."); return; }

                if (!Player.ValidName(name)) { p.SendMessage("Invalid name!"); return; }

                try
                {
                    if (p.group.Permission < LevelPermission.Admin)
                    {
                        if (x * y * z > 30000000) { p.SendMessage("Cannot create a map with over 30million blocks"); return; }
                    }
                    else
                    {
                        if (x * y * z > 225000000) { p.SendMessage("You cannot make a map with over 225million blocks"); return; }
                    }
                }
                catch { p.SendMessage("An error occured"); }

                // create a new level...
                try
                {
                    Level lvl = new Level(name, x, y, z, parameters[4]);
                    lvl.Save(true); //... and save it.
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                Player.GlobalMessage("Level " + name + " created"); // The player needs some form of confirmation.
            }
            else
                Help(p);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/newlvl - creates a new level.");
            p.SendMessage("/newlvl mapname 128 64 128 type");
            p.SendMessage("Valid types: island, mountains, forest, ocean, flat, pixel, desert");
        }
    }
}
