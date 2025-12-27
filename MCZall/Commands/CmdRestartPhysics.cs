using System;
using System.Collections.Generic;

namespace MCZall
{
    public class CmdRestartPhysics : Command
    {
        public override string Name { get { return "restartphysics"; } }
        public override string Shortcut { get { return "rp"; } }
        public override string Type { get { return "build"; } }
        public CmdRestartPhysics() { }
        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0;

            message = message.ToLower();
            cpos.extraInfo = "";

            if (message != "")
            {
                int currentLoop = 0; string[] storedArray; bool skip = false;

            retry: foreach (string s in message.Split(' '))
                {
                    if (currentLoop % 2 == 0)
                    {
                        switch (s)
                        {
                            case "drop":
                            case "explode":
                            case "dissipate":
                            case "finite":
                            case "wait":
                            case "rainbow":
                                break;
                            case "revert":
                                if (skip) break;
                                storedArray = message.Split(' ');
                                try
                                {
                                    storedArray[currentLoop + 1] = Block.Byte(message.Split(' ')[currentLoop + 1].ToString().ToLower()).ToString();
                                    if (storedArray[currentLoop + 1].ToString() == "255") throw new OverflowException();
                                }
                                catch { p.SendMessage("Invalid block type."); return; }

                                message = string.Join(" ", storedArray);
                                skip = true; currentLoop = 0;

                                goto retry;
                            default:
                                p.SendMessage(s + " is not supported."); return;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (int.Parse(s) < 1) { p.SendMessage("Values must be above 0"); return; }
                        }
                        catch { p.SendMessage("/rp [text] [num] [text] [num]"); return; }
                    }

                    currentLoop++;
                }

                if (currentLoop % 2 != 1) cpos.extraInfo = message;
                else { p.SendMessage("Number of parameters must be even"); Help(p); return; }
            }

            p.blockchangeObject = cpos;
            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/restartphysics ([type] [num]) ([type2] [num2]) (...) - Restarts every physics block in an area");
            p.SendMessage("[type] will set custom physics for selected blocks");
            p.SendMessage("Possible [types]: drop, explode, dissipate, finite, wait, rainbow, revert");
            p.SendMessage("/rp revert takes block names");
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
            List<CatchPos> buffer = new List<CatchPos>();
            CatchPos pos = new CatchPos();
            //int totalChecks = 0;

            //if (Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z) > 8000) { p.SendMessage("Tried to restart too many blocks. You may only restart 8000"); return; }

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
            {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                {
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        if (p.level.GetTile(xx, yy, zz) != Block.air)
                        {
                            pos.x = xx; pos.y = yy; pos.z = zz;
                            pos.extraInfo = cpos.extraInfo;
                            buffer.Add(pos);
                        }
                    }
                }
            }

            try
            {
                if (cpos.extraInfo == "")
                {
                    if (buffer.Count > Server.rpNormLimit)
                    {
                        p.SendMessage("Cannot restart more than " + Server.rpNormLimit + " blocks.");
                        p.SendMessage("Tried to restart " + buffer.Count + " blocks.");
                        return;
                    }
                }
                else
                {
                    if (buffer.Count > Server.rpLimit)
                    {
                        p.SendMessage("Tried to add physics to " + buffer.Count + " blocks.");
                        p.SendMessage("Cannot add physics to more than " + Server.rpLimit + " blocks.");
                        return;
                    }
                }
            }
            catch { return; }

            foreach (CatchPos pos1 in buffer)
            {
                p.level.AddCheck(p.level.PosToInt(pos1.x, pos1.y, pos1.z), pos1.extraInfo, true);
            }

            p.SendMessage("Activated " + buffer.Count + " blocks.");
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public ushort x, y, z; public string extraInfo; }
    }
}
