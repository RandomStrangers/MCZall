using System;

namespace MCZall.Commands
{
    class CmdPlace : Command
    {
        public override string Name { get { return "place"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }

        public CmdPlace() { }
        public override void Use(Player p, string message)
        {
            byte b = Block.Zero;
            ushort x; ushort y; ushort z;

            x = (ushort)(p.pos[0] / 32);
            y = (ushort)((p.pos[1] / 32) - 1);
            z = (ushort)(p.pos[2] / 32);

            try
            {
                switch (message.Split(' ').Length)
                {
                    case 0: b = Block.rock; break;
                    case 1: b = Block.Byte(message); break;
                    case 3:
                        x = Convert.ToUInt16(message.Split(' ')[0]);
                        y = Convert.ToUInt16(message.Split(' ')[1]);
                        z = Convert.ToUInt16(message.Split(' ')[2]);
                        break;
                    case 4:
                        b = Block.Byte(message.Split(' ')[0]);
                        x = Convert.ToUInt16(message.Split(' ')[1]);
                        y = Convert.ToUInt16(message.Split(' ')[2]);
                        z = Convert.ToUInt16(message.Split(' ')[3]);
                        break;
                    default: p.SendMessage("Invalid parameters"); return;
                }
            }
            catch { p.SendMessage("Invalid parameters"); return; }

            if (b == Block.Zero) b = 1;
            if (Block.AllowPlace(b) > p.group.Permission) { p.SendMessage("Cannot place that block type."); return; }

            //Level level = p.level;

            if (y >= p.level.depth) y = (ushort)(p.level.depth - 1);

            p.level.Blockchange(p, x, y, z, b);
            p.SendMessage("A block was placed at (" + x + ", " + y + ", " + z + ").");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/place [block] <x y z> - Places block at your feet or <x y z>");
        }
    }
}
