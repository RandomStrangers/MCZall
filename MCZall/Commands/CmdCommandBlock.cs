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
    public class CmdCommandBlock : Command
    {
        public override string Name { get { return "commandblock"; } }
        public override string Shortcut { get { return "cmd"; } }
        public override string Type { get { return "build"; } }
        public CmdCommandBlock() { }
        public override void Use(Player p, string message)
        {
            if (message == "" || message.Split(' ').Length < 2) { Help(p); return; }
            p.ClearBlockchange();

            CmdPos pos;

            bool b2needed = false;

            pos.cmd = all.Find(message.Split(' ')[0].ToLower()).Name;
            switch (pos.cmd)
            {
                case "cuboid":
                case "spheroid": break;
                case "replace":
                case "replacenot": b2needed = true; break;
                default: p.SendMessage("Not supported"); return;
            }

            string[] xyz = message.Split(' ');
            try
            {
                pos.x1 = Convert.ToUInt16(xyz[1]);
                pos.y1 = Convert.ToUInt16(xyz[2]);
                pos.z1 = Convert.ToUInt16(xyz[3]);
                pos.x2 = Convert.ToUInt16(xyz[4]);
                pos.y2 = Convert.ToUInt16(xyz[5]);
                pos.z2 = Convert.ToUInt16(xyz[6]);
            }
            catch { p.SendMessage("Coordinates invalid."); return; }

            pos.b1 = Block.Byte(xyz[7]);
            if (pos.b1 == Block.Zero) { p.SendMessage("Invalid block."); return; }
            if (Block.AllowPlace(pos.b1) > p.group.Permission) { p.SendMessage("Cannot use this block."); return; }

            pos.b2 = Block.Zero;
            if (b2needed)
            {
                pos.b2 = Block.Byte(xyz[8]);
                if (pos.b2 == Block.Zero) { p.SendMessage("Invalid block."); return; }
                if (Block.AllowPlace(pos.b2) > p.group.Permission) { p.SendMessage("Cannot use this block."); return; }
            }

            p.blockchangeObject = pos;
            p.Blockchange += new Player.BlockchangeEventHandler(CmdChange);
            p.SendMessage("Place a block to determine a location.");
        }

        public void CmdChange(Player p, ushort x, ushort y, ushort z, byte type)
        {

        }

        public struct CmdPos { public string cmd; public ushort x1, y1, z1, x2, y2, z2; public byte b1, b2; }

        public override void Help(Player p)
        {
            p.SendMessage("/cmd [command] [x1 y1 z1] [x2 y2 z2] [block1] [block2] - Creates a command block which performs [command] when clicked");
            p.SendMessage("Different commands require different syntax");
            p.SendMessage("Unsupported commands: ");
        }
    }
}