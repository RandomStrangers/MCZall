namespace MCZall
{
    public class CmdMute : Command
    {
        public override string Name { get { return "mute"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdMute() { }
        public override void Use(Player p, string message)
        {
            if (message == "" || message.Split(' ').Length > 2) { Help(p); return; }
            Player who = Player.Find(message);
            if (who == null) { p.SendMessage("The player entered is not online."); return; }
            if (who.muted)
            {
                who.muted = false;
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " has been &bun-muted", false);
            }
            else
            {
                if (who != p) if (who.group.Permission > p.group.Permission) { p.SendMessage("Cannot mute someone of a higher rank."); return; }
                who.muted = true;
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " has been &8muted", false);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/mute <player> - Mutes or unmutes the player.");
        }
    }
}