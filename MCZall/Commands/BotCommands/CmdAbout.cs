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
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MCZall
{
    public class CmdAbout : Command
    {
        public override string Name { get { return "about"; } }
        public override string Shortcut { get { return "b"; } }
        public override string Type { get { return "information"; } }

        public CmdAbout() { }

        public override void Use(Player p, string message)
        {
            if (message.ToLower() == "area")
            {
                CatchPos cpos;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

                p.SendMessage("Place two blocks to determine the edges.");
                p.ClearBlockchange();
                p.Blockchange += new Player.BlockchangeEventHandler(AboutBlockchange1);
            }
            else
            {
                p.SendMessage("Break/build a block to display information.");
                p.ClearBlockchange();
                p.Blockchange += new Player.BlockchangeEventHandler(AboutBlockchange);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/about [area] - Displays information about a block or [area].");
        }

        public void AboutBlockchange(Player p, ushort x, ushort y, ushort z, byte type)
        {
            if (!p.staticCommands) p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            if (b == Block.Zero) { p.SendMessage("Invalid Block(" + x + "," + y + "," + z + ")!"); return; }
            p.SendBlockchange(x, y, z, b);

            string message = "Block (" + x + "," + y + "," + z + "): ";
            message += "&f" + b + " = " + Block.Name(b);
            p.SendMessage(message + Server.DefaultColor + ".");
            message = p.level.FoundInfo(x, y, z);
            if (message != "") p.SendMessage("Physics information: &a" + message);
            p.SendMessage("Rank needed to place block: " + Group.Find(Level.PermissionToName(Block.AllowPlace(b))).Color + Level.PermissionToName(Block.AllowPlace(b)));

            DataTable Blocks = new DataTable("Block" + p.level.name);
            int totalCount = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Block" + p.level.name + " WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z, Server.mysqlCon))
                { da.Fill(Blocks); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else { Server.ErrorLog(e); return; } }


            if (Blocks.Rows.Count > 0)
            {
                string Username = Blocks.Rows[Blocks.Rows.Count - 1]["Username"].ToString();
                string TimePerformed = Blocks.Rows[Blocks.Rows.Count - 1]["TimePerformed"].ToString();
                string BlockUsed = Block.Name((byte)Blocks.Rows[Blocks.Rows.Count - 1]["Type"]).ToString();
                bool Deleted = (bool)Blocks.Rows[Blocks.Rows.Count - 1]["Deleted"];
                if (Deleted == false)
                {
                    p.SendMessage("This block was last &3created" + Server.DefaultColor + " by '" + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + "'");
                    p.SendMessage("A &3" + BlockUsed + Server.DefaultColor + " block was used.");
                    p.SendMessage("Date and time modified: &2" + TimePerformed);
                }
                else
                {
                    p.SendMessage("This block was last &4destroyed" + Server.DefaultColor + " by '" + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + "'");
                    p.SendMessage("A &3" + BlockUsed + Server.DefaultColor + " block was being held.");
                    p.SendMessage("Date and time modified: &2" + TimePerformed);
                }
            }
            else { p.SendMessage("This block has not been modified since the map was cleared."); }

            Blocks.Dispose();
        }

        public void AboutBlockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(AboutBlockchange2);
        }

        public void AboutBlockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            if (!p.staticCommands) p.ClearBlockchange();

            byte b = p.level.GetTile(x, y, z);
            if (b == Block.Zero) { p.SendMessage("Invalid Block(" + x + "," + y + "," + z + ")!"); return; }
            p.SendBlockchange(x, y, z, b);

            CatchPos cpos = (CatchPos)p.blockchangeObject;
            List<CatchPos> buffer = new List<CatchPos>();
            CatchPos pos = new CatchPos();
            ushort xx, yy, zz;

            for (xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        pos.x = xx; pos.y = yy; pos.z = zz; buffer.Add(pos);
                    }

            if (buffer.Count > 50) { p.SendMessage("Cannot check more than 50 blocks."); return; }

            List<SecondPos> foundNames = new List<SecondPos>();
            SecondPos Pos2 = new SecondPos();

            foreach (CatchPos pos1 in buffer)
            {
                DataTable Blocks = new DataTable("Block" + p.level.name);
                int totalCount = 0;
            retryTag: try
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Block" + p.level.name + " WHERE X=" + (int)pos1.x + " AND Y=" + (int)pos1.y + " AND Z=" + (int)pos1.z, Server.mysqlCon))
                    { da.Fill(Blocks); }
                }
                catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else { Server.ErrorLog(e); return; } }

                if (Blocks.Rows.Count > 0)
                {
                    string Username = Blocks.Rows[Blocks.Rows.Count - 1]["Username"].ToString();

                    Pos2.name = Username;
                    Pos2.x = pos1.x; Pos2.y = pos1.y; Pos2.z = pos1.z;

                    bool MatchFound = false;

                    foreach (SecondPos pos4 in foundNames)
                        if (pos4.name == Pos2.name) MatchFound = true;

                    if (!MatchFound) foundNames.Add(Pos2);
                }
                Blocks.Dispose();
            }

            p.SendMessage("Name | X | Y | Z");
            foreach (SecondPos Pos3 in foundNames)
            {
                p.SendMessage("&b" + Pos3.name + " | " + Pos3.x + " | " + Pos3.y + " | " + Pos3.z);
            }
        }
        struct CatchPos { public ushort x, y, z; }
        struct SecondPos { public ushort x, y, z; public string name; }
    }
}