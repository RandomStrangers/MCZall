using System;
using System.Collections.Generic;

namespace MCZall
{
    public class CmdCopy : Command
    {
        public override string Name { get { return "copy"; } }
        public override string Shortcut { get { return "c"; } }
        public override string Type { get { return "build"; } }
        public CmdCopy() { }
        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.ignoreTypes = new List<byte>();
            cpos.type = 0;
            if (message.ToLower() == "cut") { cpos.type = 1; message = ""; }
            else if (message.ToLower() == "air") { cpos.type = 2; message = ""; }
            else if (message.IndexOf(' ') != -1)
            {
                if (message.Split(' ')[0] == "ignore")
                {
                    foreach (string s in message.Substring(message.IndexOf(' ') + 1).Split(' '))
                    {
                        if (Block.Byte(s) != Block.Zero)
                        {
                            cpos.ignoreTypes.Add(Block.Byte(s));
                            p.SendMessage("Ignoring &b" + s);
                        }
                    }
                }
                else { Help(p); return; }
                message = "";
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            if (message != "") { Help(p); return; }

            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/copy - Copies the blocks in an area.");
            p.SendMessage("/copy cut - Copies the blocks in an area, then removes them.");
            p.SendMessage("/copy air - Copies the blocks in an area, including air.");
            p.SendMessage("/copy ignore <block1> <block2>.. - Ignores <blocks> when copying");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            p.CopyBuffer.Clear();
            int TotalAir = 0;
            if (cpos.type == 2) p.copyAir = true; else p.copyAir = false;

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        b = p.level.GetTile(xx, yy, zz);
                        if (Block.AllowPlace(b) <= p.group.Permission)
                        {
                            if (b == Block.air && cpos.type != 2 || cpos.ignoreTypes.Contains(b)) TotalAir++;

                            if (cpos.ignoreTypes.Contains(b)) BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), Block.air);
                            else BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), b);
                        }
                        else BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), Block.air);
                    }

            if ((p.CopyBuffer.Count - TotalAir) > p.group.MaxBlocks)
            {
                p.SendMessage("You tried to copy " + p.CopyBuffer.Count + " blocks.");
                p.SendMessage("You cannot copy more than " + p.group.MaxBlocks + ".");
                p.CopyBuffer.Clear();
                return;
            }

            if (cpos.type == 1)
                for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            b = p.level.GetTile(xx, yy, zz);
                            if (b != Block.air && Block.AllowPlace(b) <= p.group.Permission)
                                p.level.Blockchange(p, xx, yy, zz, Block.air);
                        }

            p.SendMessage(p.CopyBuffer.Count - TotalAir + " blocks copied.");
        }

        void BufferAdd(Player p, ushort x, ushort y, ushort z, byte type)
        {
            Player.CopyPos pos; pos.x = x; pos.y = y; pos.z = z; pos.type = type;
            p.CopyBuffer.Add(pos);
        }
        struct CatchPos { public ushort x, y, z; public int type; public List<byte> ignoreTypes; }
    }
}