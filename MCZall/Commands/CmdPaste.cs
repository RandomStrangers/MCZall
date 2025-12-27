using System;

namespace MCZall
{
    public class CmdPaste : Command
    {
        public override string Name { get { return "paste"; } }
        public override string Shortcut { get { return "v"; } }
        public override string Type { get { return "build"; } }
        public CmdPaste() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            p.SendMessage("Place a block in the corner of where you want to paste."); p.ClearBlockchange();
            p.SendMessage(">&4BEWARE: " + Server.DefaultColor + "The blocks will always be pasted in a set direction");
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/paste - Pastes the stored copy.");
            p.SendMessage(">&4BEWARE: " + Server.DefaultColor + "The blocks will always be pasted in a set direction");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);

            Player.UndoPos Pos1;
            //p.UndoBuffer.Clear();
            p.CopyBuffer.ForEach(delegate (Player.CopyPos pos)
            {
                Pos1.x = (ushort)(Math.Abs(pos.x) + x);
                Pos1.y = (ushort)(Math.Abs(pos.y) + y);
                Pos1.z = (ushort)(Math.Abs(pos.z) + z);

                if (pos.type != Block.air || p.copyAir)
                    unchecked { if (p.level.GetTile(Pos1.x, Pos1.y, Pos1.z) != Block.Zero) p.level.Blockchange(p, Pos1.x, Pos1.y, Pos1.z, pos.type); }
            });

            p.SendMessage("Pasted " + p.CopyBuffer.Count + " blocks.");

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public ushort x, y, z; }
    }
}