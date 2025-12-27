namespace MCZall
{
    public class CmdImport : Command
    {
        public override string Name { get { return "import"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdImport() { }
        public override void Use(Player p, string message)
        {
        }
        public override void Help(Player p)
        {
            p.SendMessage("/import [mapname] - Imports a .dat map.");
        }
    }
}