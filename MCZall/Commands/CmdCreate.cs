namespace MCZall
{
    public class CmdCreate : Command
    {
        public override string Name { get { return "create"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdCreate() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length != 2) { Help(p); return; }

            CatchPos cpos = new CatchPos();

            switch (message.Split(' ')[0].ToLower())
            {
                case "mountain":
                case "hill":
                case "lake":
                    cpos.type = message.Split(' ')[0].ToLower();
                    break;

                default:
                    p.SendMessage("No type stored by this name.");
                    return;
            }

            try { cpos.size = int.Parse(message.Split(' ')[1]); } catch { p.SendMessage("Invalid size."); return; }

            p.SendMessage("Place block to determine the edges.");
            p.ClearBlockchange();
            p.blockchangeObject = cpos;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/create [object] [size] - Creates [object] that's [size] big.");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);

            //CatchPos cpos = (CatchPos)p.blockchangeObject;


        }

        public struct CatchPos { public string type; public int size; }
    }
}