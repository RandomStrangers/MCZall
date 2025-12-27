namespace MCZall
{
    public class CmdFill : Command
    {
        public override string Name { get { return "fill"; } }
        public override string Shortcut { get { return "f"; } }
        public override string Type { get { return "build"; } }
        public CmdFill() { }
        public override void Use(Player p, string message)
        {
            CatchPos cpos;

            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                cpos.type = Block.Byte(t);
                if (cpos.type == 255) { p.SendMessage("There is no block \"" + t + "\"."); return; }

                if (Block.AllowPlace(cpos.type) > p.group.Permission) { p.SendMessage("Cannot place that."); return; }

                if (s == "up") cpos.FillType = 1;
                else if (s == "down") cpos.FillType = 2;
                else if (s == "layer") cpos.FillType = 3;
                else if (s == "vertical_x") cpos.FillType = 4;
                else if (s == "vertical_z") cpos.FillType = 5;
                else { p.SendMessage("Invalid fill type"); return; }
            }
            else if (message != "")
            {
                message = message.ToLower();

                if (message == "up") { cpos.FillType = 1; cpos.type = Block.Zero; }
                else if (message == "down") { cpos.FillType = 2; cpos.type = Block.Zero; }
                else if (message == "layer") { cpos.FillType = 3; cpos.type = Block.Zero; }
                else if (message == "vertical_x") { cpos.FillType = 4; cpos.type = Block.Zero; }
                else if (message == "vertical_z") { cpos.FillType = 5; cpos.type = Block.Zero; }
                else
                {
                    cpos.type = Block.Byte(message);
                    if (cpos.type == 255) { p.SendMessage("Invalid block or fill type"); return; }
                    if (Block.AllowPlace(cpos.type) > p.group.Permission) { p.SendMessage("Cannot place that."); return; }

                    cpos.FillType = 0;
                }
            }
            else
            {
                cpos.type = Block.Zero; cpos.FillType = 0;
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            p.SendMessage("Destroy the block you wish to fill."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/fill [block] [type] - Fills the area specified with [block].");
            p.SendMessage("[types] - up, down, layer, vertical_x, vertical_z");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            if (cpos.type == Block.Zero) cpos.type = p.bindings[type];

            byte oldType = p.level.GetTile(x, y, z);
            if (Block.AllowPlace(oldType) > p.group.Permission && !Block.BuildIn(oldType)) { p.SendMessage("Cannot fill that."); return; }

            int totalFilled = 0; int Counter = 0;

            if (FloodFill(p, x, y, z, cpos.type, oldType, cpos.FillType, ref totalFilled) == false)
            {
                try
                {
                    for (int i = p.UndoBuffer.Count - 1; i > p.UndoBuffer.Count - totalFilled; --i)
                    {
                        Counter++;

                        x = p.UndoBuffer[i].x;
                        y = p.UndoBuffer[i].y;
                        z = p.UndoBuffer[i].z;

                        if (p.level.GetTile(x, y, z) == cpos.type)
                            p.level.Blockchange(x, y, z, oldType, true);
                        p.UndoBuffer.RemoveAt(i);

                        if (Counter > totalFilled) return;
                    }
                }
                catch { }
                return;
            }

            p.SendMessage("Filled " + totalFilled + " blocks.");

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        public bool FloodFill(Player p, ushort x, ushort y, ushort z, byte b, byte oldType, int fillType, ref int totalFilled)
        {
            totalFilled++;
            Pos pos;

            p.level.Blockchange(p, x, y, z, b);

            if (p.group.Permission < LevelPermission.AdvBuilder)
            {
                p.SendMessage("You cannot use this command.");
                return false;
            }
            else if (p.group.Permission == LevelPermission.AdvBuilder && totalFilled > 1200)
            {
                p.SendMessage("You tried to fill " + totalFilled + " blocks.");
                p.SendMessage("You can only fill 1200 blocks");
                return false;
            }
            else if (p.group.Permission == LevelPermission.Operator && totalFilled > 2500)
            {
                p.SendMessage("You tried to fill " + totalFilled + " blocks.");
                p.SendMessage("You can only fill 2500 blocks");
                return false;
            }
            else if (totalFilled > 5000)
            {
                p.SendMessage("You tried to fill " + totalFilled + " blocks.");
                p.SendMessage("You can only fill 5000 blocks.");
                return false;
            }

            //x
            if (fillType != 4)
            {
                pos.x = (ushort)(x + 1); pos.y = y; pos.z = z;
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;

                pos.x = (ushort)(x - 1); pos.y = y; pos.z = z;
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;
            }

            //z
            if (fillType != 5)
            {
                pos.x = x; pos.y = y; pos.z = (ushort)(z + 1);
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;

                pos.x = x; pos.y = y; pos.z = (ushort)(z - 1);
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;
            }

            //y
            if (fillType == 0 || fillType == 1 || fillType > 3)
            {
                pos.x = x; pos.y = (ushort)(y + 1); pos.z = z;
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;
            }

            if (fillType == 0 || fillType == 2 || fillType > 3)
            {
                pos.x = x; pos.y = (ushort)(y - 1); pos.z = z;
                if (p.level.GetTile(pos.x, pos.y, pos.z) == oldType)
                    if (FloodFill(p, pos.x, pos.y, pos.z, b, oldType, fillType, ref totalFilled) == false) return false;
            }

            return true;
        }

        struct CatchPos { public ushort x, y, z; public byte type; public int FillType; }
        struct Pos { public ushort x, y, z; }
    }
}