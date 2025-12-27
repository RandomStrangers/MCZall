namespace MCZall
{
    public class CmdFollow : Command
    {
        public override string Name { get { return "follow"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdFollow() { }
        public override void Use(Player p, string message)
        {
            if (message == "" && p.following == "") { Help(p); return; }
            else if (message == "" && p.following != "" || message == p.following) { p.following = ""; if (p.hidden == true) all.Find("hide").Use(p, ""); return; }

            Player who = Player.Find(message);
            if (who == null) { p.SendMessage("Could not find player."); return; }
            else if (who == p) { p.SendMessage("Cannot follow yourself."); return; }
            else if (who.group.Permission >= p.group.Permission) { p.SendMessage("Cannot follow someone of equal or greater rank."); return; }
            else if (who.following != "") { p.SendMessage(who.name + " is already following " + who.following); return; }

            if (!p.hidden) all.Find("hide").Use(p, "");

            if (p.level != who.level) all.Find("tp").Use(p, who.name);
            p.following = who.name;
            p.SendMessage("Following " + who.name + ". Use \"/follow\" to stop.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/follow <name> - Follows <name> until the command is cancelled.");
        }
    }
}