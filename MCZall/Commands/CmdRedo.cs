namespace MCZall
{
    public class CmdRedo : Command
    {
        public override string Name { get { return "redo"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdRedo() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            byte b;

            p.RedoBuffer.ForEach(delegate (Player.UndoPos Pos)
            {
                b = Pos.map.GetTile(Pos.x, Pos.y, Pos.z);
                Pos.map.Blockchange(Pos.x, Pos.y, Pos.z, Pos.type);
                Pos.type = b; p.UndoBuffer.Add(Pos);
            });

            p.SendMessage("Redo performed.");
        }

        public override void Help(Player p)
        {
            p.SendMessage("/redo - Redoes the Undo you just performed.");
        }
    }
}