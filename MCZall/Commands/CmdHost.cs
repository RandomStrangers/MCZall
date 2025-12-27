namespace MCZall
{
    public class CmdHost : Command
    {
        public override string Name { get { return "host"; } }
        public override string Shortcut { get { return "zall"; } }
        public override string Type { get { return "information"; } }
        public CmdHost() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            p.SendMessage("Host is currently &3" + Server.ZallState + ".");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/host - Shows what the host is up to.");
        }
    }
}