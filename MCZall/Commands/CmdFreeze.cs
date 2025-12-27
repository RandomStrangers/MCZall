namespace MCZall
{
    public class CmdFreeze : Command
    {
        public override string Name { get { return "freeze"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdFreeze() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message);
            if (who == null) { p.SendMessage("Could not find player."); return; }
            else if (who == p) { p.SendMessage("Cannot freeze yourself."); return; }
            else if (who.group.Permission >= p.group.Permission) { p.SendMessage("Cannot freeze someone of equal or greater rank."); return; }

            if (!who.frozen)
            {
                who.frozen = true;
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " has been &bfrozen.", false);
            }
            else
            {
                who.frozen = false;
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " has been &adefrosted.", false);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/freeze <name> - Stops <name> from moving until unfrozen.");
        }
    }
}