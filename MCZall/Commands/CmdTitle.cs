namespace MCZall
{
    public class CmdTitle : Command
    {
        public override string Name { get { return "title"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdTitle() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length < 1) { Help(p); return; }

            //int pos = message.IndexOf(' ');
            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { p.SendMessage("Could not find player."); return; }

            string newTitle;
            if (message.Split(' ').Length > 1) newTitle = message.Substring(message.IndexOf(' ') + 1);
            else { who.prefix = ""; Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " had their title removed.", false); return; }


            if (newTitle != "")
            {
                if (newTitle[0].ToString() != "[") newTitle = "[" + newTitle;
                if (newTitle.Trim()[newTitle.Trim().Length - 1].ToString() != "]") newTitle = newTitle.Trim() + "]";
                if (newTitle[newTitle.Length - 1].ToString() != " ") newTitle += " ";
            }

            if (newTitle.Length > 20) { p.SendMessage("Title must be under 17 letters."); return; }
            if (who.name == "Zallist" || newTitle.ToLower() == "[dev] ") { p.SendMessage("Can't let you do that, starfox."); return; }

            if (newTitle != "")
                Player.GlobalChat(null, who.color + who.name + Server.DefaultColor + " was given the title of &b" + newTitle, false);
            else Player.GlobalChat(null, who.color + who.prefix + who.name + Server.DefaultColor + " had their title removed.", false);

            who.prefix = newTitle;

        }
        public override void Help(Player p)
        {
            p.SendMessage("/title [player] [title] - Gives [player] the [title].");
            p.SendMessage("If no [title] is given, the player's title is removed.");
        }
    }
}
