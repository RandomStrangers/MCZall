namespace MCZall
{
    public class CmdLimit : Command
    {
        public override string Name { get { return "limit"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdLimit() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length != 2) { Help(p); return; }
            int newLimit;
            try { newLimit = int.Parse(message.Split(' ')[1]); } catch { p.SendMessage("Invalid limit amount"); return; }
            if (newLimit < 1) { p.SendMessage("Cannot set below 1."); return; }

            Group foundGroup = Group.Find(message.Split(' ')[0]);
            if (foundGroup != null)
            {
                if (foundGroup.Name == "superop") Server.maxSuper = newLimit;
                else if (foundGroup.Name == "operator") Server.maxOp = newLimit;
                else if (foundGroup.Name == "advbuilder") Server.maxAdv = newLimit;
                else if (foundGroup.Name == "builder") Server.maxBuild = newLimit;

                Player.GlobalChat(null, foundGroup.Color + foundGroup.Name + Server.DefaultColor + "'s building limits were set to &b" + newLimit, false);
            }
            else
            {
                switch (message.Split(' ')[0].ToLower())
                {
                    case "rp":
                    case "restartphysics":
                        Server.rpLimit = newLimit;
                        Player.GlobalMessage("Custom /rp's limit was changed to &b" + newLimit.ToString());
                        break;
                    case "rpnorm":
                    case "rpnormal":
                        Server.rpNormLimit = newLimit;
                        Player.GlobalMessage("Normal /rp's limit was changed to &b" + newLimit.ToString());
                        break;

                    default:
                        p.SendMessage("No supported /limit");
                        break;
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/limit <type> <amount> - Sets the limit for <type>");
            p.SendMessage("<types> - Builder, AdvBuilder, Operator, SuperOP, RP, RPNormal");
        }
    }
}