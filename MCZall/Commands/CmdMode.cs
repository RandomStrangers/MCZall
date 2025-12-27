namespace MCZall
{
    public class CmdMode : Command
    {
        public override string Name { get { return "mode"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdMode() { }
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.modeType != 0)
                {
                    p.SendMessage("&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.BlockAction = 0;
                }
                else
                {
                    Help(p); return;
                }
            }
            else
            {
                byte b = Block.Byte(message);
                if (b == Block.Zero) { p.SendMessage("Could not find block given."); return; }
                if (b == Block.air) { p.SendMessage("Cannot use Air Mode."); return; }
                if (Block.AllowPlace(b) > p.group.Permission) { p.SendMessage("Cannot place this block at your rank."); return; }

                if (p.modeType == b)
                {
                    p.SendMessage("&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.BlockAction = 0;
                }
                else
                {
                    p.BlockAction = 6;
                    p.modeType = b;
                    p.SendMessage("&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &aON");
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/mode [block] - Makes every block placed into [block].");
            p.SendMessage("/[block] also works");
        }
    }
}