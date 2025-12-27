namespace MCZall
{
    public class CmdJail : Command
    {
        public override string Name { get { return "jail"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdJail() { }
        public override void Use(Player p, string message)
        {
            if ((message.ToLower() == "create" || message.ToLower() == "") && p != null)
            {
                p.level.jailx = p.pos[0]; p.level.jaily = p.pos[1]; p.level.jailz = p.pos[2];
                p.level.jailrotx = p.rot[0]; p.level.jailroty = p.rot[1];
                p.SendMessage("Set Jail point.");
            }
            else
            {
                Player who = Player.Find(message);
                if (who != null)
                {
                    if (!who.jailed)
                    {
                        if (p != null) if (who.group.Permission >= p.group.Permission) { p.SendMessage("Cannot jail someone of equal or greater rank."); return; }
                        if (who.level != p.level) all.Find("goto").Use(who, p.level.name);
                        Player.GlobalDie(who, false);
                        Player.GlobalSpawn(who, p.level.jailx, p.level.jaily, p.level.jailz, p.level.jailrotx, p.level.jailroty, true);
                        who.jailed = true;
                        Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " was &8jailed", false);
                    }
                    else
                    {
                        who.jailed = false;
                        Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " was &afreed" + Server.DefaultColor + " from jail", false);
                    }
                }
                else
                {
                    p.SendMessage("Could not find specified player.");
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/jail [user] - Places [user] in jail unable to use commands.");
            p.SendMessage("/jail [create] - Creates the jail point for the map.");
        }
    }
}