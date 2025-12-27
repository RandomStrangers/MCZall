using System;
using System.Collections.Generic;

namespace MCZall
{
    public class CmdLine : Command
    {
        public override string Name { get { return "line"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdLine() { }
        public override void Use(Player p, string message)
        {
            CatchPos cpos;

            if (message == "")
            {
                cpos.maxNum = 0;
                cpos.wall = false;
                cpos.type = Block.Zero;
            }
            else if (message.IndexOf(' ') == -1)
            {
                try
                {
                    cpos.maxNum = int.Parse(message);
                    cpos.wall = false;
                    cpos.type = Block.Zero;
                }
                catch
                {
                    cpos.maxNum = 0;
                    if (message == "wall")
                    {
                        cpos.wall = true;
                        cpos.type = Block.Zero;
                    }
                    else
                    {
                        cpos.wall = false;
                        cpos.type = Block.Byte(message);
                        if (cpos.type == Block.Zero)
                        {
                            Help(p); return;
                        }
                    }
                }
            }
            else
            {
                if (message.Split(' ').Length == 2)
                {
                    try
                    {
                        cpos.maxNum = int.Parse(message.Split(' ')[0]);
                        cpos.type = Block.Byte(message.Split(' ')[1]);
                        if (cpos.type == Block.Zero) if (message.Split(' ')[1] == "wall") cpos.wall = true; else cpos.wall = false; else cpos.wall = false;
                    }
                    catch
                    {
                        cpos.maxNum = 0;
                        cpos.type = Block.Byte(message.Split(' ')[0]); if (cpos.type == Block.Zero) { Help(p); return; }
                        if (message.Split(' ')[1] == "wall") cpos.wall = true; else cpos.wall = false;
                    }
                }
                else
                {
                    try { cpos.maxNum = int.Parse(message.Split(' ')[0]); } catch { Help(p); return; }
                    cpos.type = Block.Byte(message.Split(' ')[1]); if (cpos.type == Block.Zero) { Help(p); return; }
                    if (message.Split(' ')[2] == "wall") cpos.wall = true; else cpos.wall = false;
                }
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/line [num] <block> [wall] - Creates a line between two blocks [num] long.");
            p.SendMessage("If \"wall\" is added, a cuboid-like wall will be made");
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
            if (cpos.type == Block.Zero) type = p.bindings[type]; else type = cpos.type;
            List<CatchPos> buffer = new List<CatchPos>();
            CatchPos pos = new CatchPos();

            if (cpos.maxNum == 0) cpos.maxNum = 100000;

            int i, dx, dy, dz, l, m, n, x_inc, y_inc, z_inc, err_1, err_2, dx2, dy2, dz2;
            int[] pixel = new int[3];

            pixel[0] = cpos.x; pixel[1] = cpos.y; pixel[2] = cpos.z;
            dx = x - cpos.x; dy = y - cpos.y; dz = z - cpos.z;

            x_inc = (dx < 0) ? -1 : 1; l = Math.Abs(dx);
            y_inc = (dy < 0) ? -1 : 1; m = Math.Abs(dy);
            z_inc = (dz < 0) ? -1 : 1; n = Math.Abs(dz);

            dx2 = l << 1; dy2 = m << 1; dz2 = n << 1;

            if ((l >= m) && (l >= n))
            {
                err_1 = dy2 - l;
                err_2 = dz2 - l;
                for (i = 0; i < l; i++)
                {
                    pos.x = (ushort)pixel[0];
                    pos.y = (ushort)pixel[1];
                    pos.z = (ushort)pixel[2];
                    buffer.Add(pos);

                    if (err_1 > 0)
                    {
                        pixel[1] += y_inc;
                        err_1 -= dx2;
                    }
                    if (err_2 > 0)
                    {
                        pixel[2] += z_inc;
                        err_2 -= dx2;
                    }
                    err_1 += dy2;
                    err_2 += dz2;
                    pixel[0] += x_inc;
                }
            }
            else if ((m >= l) && (m >= n))
            {
                err_1 = dx2 - m;
                err_2 = dz2 - m;
                for (i = 0; i < m; i++)
                {
                    pos.x = (ushort)pixel[0];
                    pos.y = (ushort)pixel[1];
                    pos.z = (ushort)pixel[2];
                    buffer.Add(pos);

                    if (err_1 > 0)
                    {
                        pixel[0] += x_inc;
                        err_1 -= dy2;
                    }
                    if (err_2 > 0)
                    {
                        pixel[2] += z_inc;
                        err_2 -= dy2;
                    }
                    err_1 += dx2;
                    err_2 += dz2;
                    pixel[1] += y_inc;
                }
            }
            else
            {
                err_1 = dy2 - n;
                err_2 = dx2 - n;
                for (i = 0; i < n; i++)
                {
                    pos.x = (ushort)pixel[0];
                    pos.y = (ushort)pixel[1];
                    pos.z = (ushort)pixel[2];
                    buffer.Add(pos);

                    if (err_1 > 0)
                    {
                        pixel[1] += y_inc;
                        err_1 -= dz2;
                    }
                    if (err_2 > 0)
                    {
                        pixel[0] += x_inc;
                        err_2 -= dz2;
                    }
                    err_1 += dy2;
                    err_2 += dx2;
                    pixel[2] += z_inc;
                }
            }

            pos.x = (ushort)pixel[0];
            pos.y = (ushort)pixel[1];
            pos.z = (ushort)pixel[2];
            buffer.Add(pos);

            int count;
            count = Math.Min(buffer.Count, cpos.maxNum);
            if (cpos.wall) count *= Math.Abs(cpos.y - y);

            if (count > p.group.MaxBlocks)
            {
                p.SendMessage("You tried to fill " + count + " blocks at once.");
                p.SendMessage("You are limited to " + p.group.MaxBlocks);
                return;
            }

            for (count = 0; count < cpos.maxNum && count < buffer.Count; count++)
            {
                if (!cpos.wall)
                {
                    p.level.Blockchange(p, buffer[count].x, buffer[count].y, buffer[count].z, type);
                }
                else
                {
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++)
                    {
                        p.level.Blockchange(p, buffer[count].x, yy, buffer[count].z, type);
                    }
                }
            }

            p.SendMessage("Line was " + count.ToString() + " blocks long.");

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        struct CatchPos { public ushort x, y, z; public int maxNum; public bool wall; public byte type; }
    }
}