namespace MCZall
{
    public class CmdBots : Command
    {
        public override string Name { get { return "bots"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdBots() { }
        public override void Use(Player p, string message)
        {
            message = "";
            foreach (PlayerBot Pb in PlayerBot.playerbots)
            {
                if (Pb.AIName != "") message += ", " + Pb.name + "(" + Pb.level.name + ")[" + Pb.AIName + "]";
                else if (Pb.hunt) message += ", " + Pb.name + "(" + Pb.level.name + ")[Hunt]";
                else message += ", " + Pb.name + "(" + Pb.level.name + ")";

                if (Pb.kill) message += "-kill";
            }

            if (message != "") p.SendMessage("&1Bots: " + Server.DefaultColor + message.Remove(0, 2));
            else p.SendMessage("No bots are alive.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/bots - Shows a list of bots, their AIs and levels");
        }
    }
}