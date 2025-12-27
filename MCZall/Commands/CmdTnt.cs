namespace MCZall
{
    public class CmdTnt : Command
    {
        public override string Name { get { return "tnt"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdTnt() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length > 1) { Help(p); return; }

            if (p.BlockAction == 13 || p.BlockAction == 14)
            {
                p.BlockAction = 0; p.SendMessage("TNT mode is now &cOFF" + Server.DefaultColor + ".");
            }
            else if (message.ToLower() == "small" || message == "")
            {
                p.BlockAction = 13; p.SendMessage("TNT mode is now &aON" + Server.DefaultColor + ".");
            }
            else if (message.ToLower() == "big")
            {
                if (Server.operators.Contains(p.name) || Server.superOps.Contains(p.name))
                {
                    p.BlockAction = 14; p.SendMessage("TNT mode is now &aON" + Server.DefaultColor + ".");
                }
                else
                {
                    p.SendMessage("This mode is reserved for OPs");
                }
            }
            else
            {
                Help(p);
            }

            p.painting = false;
        }
        public override void Help(Player p)
        {
            p.SendMessage("/tnt [small/big] - Creates exploding TNT (with Physics 3).");
            p.SendMessage("Big TNT is reserved for OP+.");
        }
    }
}