using System.Collections.Generic;

namespace MCZall
{
    public class CmdSpin : Command
    {
        public override string Name { get { return "spin"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdSpin() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length > 1) { Help(p); return; }
            if (message == "") message = "90";

            List<Player.CopyPos> newBuffer = new List<Player.CopyPos>();
            int TotalLoop = 0; ushort temp;
            newBuffer.Clear();

            switch (message)
            {
                case "90":
                    p.CopyBuffer.ForEach(delegate (Player.CopyPos Pos)
                    {
                        temp = Pos.z; Pos.z = Pos.x; Pos.x = temp;
                        p.CopyBuffer[TotalLoop] = Pos;
                        TotalLoop += 1;
                    });
                    goto case "m";
                case "180":
                    TotalLoop = p.CopyBuffer.Count;
                    p.CopyBuffer.ForEach(delegate (Player.CopyPos Pos)
                    {
                        TotalLoop -= 1;
                        Pos.x = p.CopyBuffer[TotalLoop].x;
                        Pos.z = p.CopyBuffer[TotalLoop].z;
                        newBuffer.Add(Pos);
                    });
                    p.CopyBuffer.Clear();
                    p.CopyBuffer = newBuffer;
                    break;
                case "upsidedown":
                case "u":
                    TotalLoop = p.CopyBuffer.Count;
                    p.CopyBuffer.ForEach(delegate (Player.CopyPos Pos)
                    {
                        TotalLoop -= 1;
                        Pos.y = p.CopyBuffer[TotalLoop].y;
                        newBuffer.Add(Pos);
                    });
                    p.CopyBuffer.Clear();
                    p.CopyBuffer = newBuffer;
                    break;
                case "mirror":
                case "m":
                    TotalLoop = p.CopyBuffer.Count;
                    p.CopyBuffer.ForEach(delegate (Player.CopyPos Pos)
                    {
                        TotalLoop -= 1;
                        Pos.x = p.CopyBuffer[TotalLoop].x;
                        newBuffer.Add(Pos);
                    });
                    p.CopyBuffer.Clear();
                    p.CopyBuffer = newBuffer;
                    break;
                default:
                    p.SendMessage("Incorrect syntax"); Help(p);
                    return;
            }

            p.SendMessage("Spun: &b" + message);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/spin <90/180/mirror/upsidedown> - Spins the copied object.");
            p.SendMessage("Shotcuts: m for mirror and u for upsidedown");
        }
    }
}