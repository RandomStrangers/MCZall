using System;
using System.IO;

namespace MCZall
{
    public class CmdBotSet : Command
    {
        public override string Name { get { return "botset"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdBotSet() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            try
            {
                if (message.Split(' ').Length == 1)
                {
                    PlayerBot pB = PlayerBot.Find(message);
                    try { pB.Waypoints.Clear(); } catch { }
                    pB.kill = false;
                    pB.hunt = false;
                    pB.AIName = "";
                    p?.SendMessage(pB.color + pB.name + Server.DefaultColor + "'s AI was turned off.");
                    return;
                }
                else if (message.Split(' ').Length != 2)
                {
                    if (p != null) Help(p); return;
                }

                PlayerBot Pb = PlayerBot.Find(message.Split(' ')[0]);
                if (Pb == null) { p?.SendMessage("Could not find specified Bot"); return; }
                string foundPath = message.Split(' ')[1].ToLower();

                if (foundPath == "hunt")
                {
                    Pb.hunt = !Pb.hunt;
                    try { Pb.Waypoints.Clear(); } catch { }
                    Pb.AIName = "";
                    if (p != null) Player.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s hunt instinct: " + Pb.hunt, false);
                    return;
                }
                else if (foundPath == "kill")
                {
                    if (p.group.Permission < LevelPermission.Operator) { p.SendMessage("Only an OP may toggle killer instinct."); return; }
                    Pb.kill = !Pb.kill;
                    if (p != null) Player.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s kill instinct: " + Pb.kill, false);
                    return;
                }

                if (!File.Exists("bots/" + foundPath)) { p?.SendMessage("Could not find specified AI."); return; }

                string[] foundWay = File.ReadAllLines("bots/" + foundPath);

                if (foundWay[0] != "#Version 2") { p?.SendMessage("Invalid file version. Remake"); return; }

                PlayerBot.Pos newPos = new PlayerBot.Pos();
                try { Pb.Waypoints.Clear(); Pb.currentPoint = 0; Pb.countdown = 0; Pb.movementSpeed = 12; } catch { }

                try
                {
                    foreach (string s in foundWay)
                    {
                        if (s != "" && s[0] != '#')
                        {
                            bool skip = false;
                            newPos.type = s.Split(' ')[0];
                            switch (s.Split(' ')[0].ToLower())
                            {
                                case "walk":
                                case "teleport":
                                    newPos.x = Convert.ToUInt16(s.Split(' ')[1]);
                                    newPos.y = Convert.ToUInt16(s.Split(' ')[2]);
                                    newPos.z = Convert.ToUInt16(s.Split(' ')[3]);
                                    newPos.rotx = Convert.ToByte(s.Split(' ')[4]);
                                    newPos.roty = Convert.ToByte(s.Split(' ')[5]);
                                    break;
                                case "wait":
                                case "speed":
                                    newPos.seconds = Convert.ToInt16(s.Split(' ')[1]); break;
                                case "nod":
                                case "spin":
                                    newPos.seconds = Convert.ToInt16(s.Split(' ')[1]);
                                    newPos.rotspeed = Convert.ToInt16(s.Split(' ')[2]);
                                    break;
                                case "linkscript":
                                    newPos.newscript = s.Split(' ')[1]; break;
                                case "reset":
                                case "jump":
                                case "remove": break;
                                default: skip = true; break;
                            }
                            if (!skip) Pb.Waypoints.Add(newPos);
                        }
                    }
                }
                catch { p?.SendMessage("AI file corrupt."); return; }

                Pb.AIName = foundPath;
                if (p != null) Player.GlobalChatLevel(p, Pb.color + Pb.name + Server.DefaultColor + "'s AI is now set to " + foundPath, false);
            }
            catch { p?.SendMessage("Error"); return; }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/botset <bot> <AI script> - Makes <bot> do <AI script>");
            p.SendMessage("Special AI scripts: Kill and Hunt");
        }
    }
}