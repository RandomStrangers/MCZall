namespace MCZall
{
    public class CmdDelete : Command
    {
        public override string Name { get { return "delete"; } }
        public override string Shortcut { get { return "d"; } }
        public override string Type { get { return "build"; } }
        public CmdDelete() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            p.deleteMode = !p.deleteMode;
            p.SendMessage("Delete mode: &a" + p.deleteMode);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/delete - Deletes any block you click");
            p.SendMessage("\"any block\" meaning door_air, portals, mb's, etc");
        }
    }
}