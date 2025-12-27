namespace MCZall
{
    public class CmdInvincible : Command
    {
        public override string Name { get { return "invincible"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdInvincible() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            if (p.invincible == true)
            {
                p.invincible = false;
                if (Server.cheapMessage)
                    Player.GlobalChat(p, p.color + p.name + Server.DefaultColor + " has stopped being cheap and is now mortal", false);
            }
            else
            {
                p.invincible = true;
                if (Server.cheapMessage)
                    Player.GlobalChat(p, p.color + p.name + Server.DefaultColor + " is now being cheap and being immortal", false);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/invincible - Turns invincible mode on/off.");
        }
    }
}