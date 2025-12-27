using System;

namespace MCZall
{
    public class CmdUndo : Command
    {
        public override string Name { get { return "undo"; } }
        public override string Shortcut { get { return "u"; } }
        public override string Type { get { return "build"; } }
        public CmdUndo() { }
        public override void Use(Player p, string message)
        {
            byte b; long seconds; Player who; Player.UndoPos Pos; int CurrentPos;
            p?.RedoBuffer.Clear();

            if (message == "") message = p.name + " 30";

            if (message.Split(' ').Length == 2)
            {
                if (message.Split(' ')[1].ToLower() == "all" && p.group.Permission > LevelPermission.Operator)
                {
                    seconds = 500000;
                }
                else
                {
                    try { seconds = long.Parse(message.Split(' ')[1]); } catch { p.SendMessage("Invalid seconds."); return; }
                }
            }
            else
            {
                try { seconds = int.Parse(message); message = p.name + " " + message; }
                catch { seconds = 30; message += " 30"; }
            }

            //if (message.Split(' ').Length == 1) if (char.IsDigit(message, 0)) { message = p.name + " " + message; } else { message = message + " 30"; }

            //try { seconds = Convert.ToInt16(message.Split(' ')[1]); } catch { seconds = 2; }
            if (seconds == 0) seconds = 1800;

            who = Player.Find(message.Split(' ')[0]);
            if (who != null)
            {
                if (p != null)
                {
                    if (who.group.Permission > p.group.Permission && who != p) { p.SendMessage("Cannot undo a user of higher or equal rank"); return; }
                    if (who != p && p.group.Permission != LevelPermission.Admin && p.group.Permission != LevelPermission.Operator) { p.SendMessage("Only an OP+ may undo other people's actions"); return; }

                    if (p.group.Permission == LevelPermission.Guest && seconds > 120) { p.SendMessage("Guests may only undo 2 minutes."); return; }
                    else if (p.group.Permission == LevelPermission.Builder && seconds > 300) { p.SendMessage("Builders may only undo 300 seconds."); return; }
                    else if (p.group.Permission == LevelPermission.AdvBuilder && seconds > 1200) { p.SendMessage("AdvBuilders may only undo 600 seconds."); return; }
                    else if (p.group.Permission == LevelPermission.Operator && seconds > 5400) { p.SendMessage("Operators may only undo 5400 seconds."); return; }
                }

                for (CurrentPos = who.UndoBuffer.Count - 1; CurrentPos >= 0; --CurrentPos)
                {
                    try
                    {
                        Pos = who.UndoBuffer[CurrentPos];
                        b = Pos.map.GetTile(Pos.x, Pos.y, Pos.z);
                        if (Pos.timePlaced.AddSeconds(seconds) >= DateTime.Now)
                        {
                            if (b == Pos.newtype || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            {
                                Pos.map.Blockchange(Pos.x, Pos.y, Pos.z, Pos.type, true);

                                Pos.newtype = Pos.type; Pos.type = b;
                                p?.RedoBuffer.Add(Pos);
                                who.UndoBuffer.RemoveAt(CurrentPos);
                            }
                        }
                        else break;
                    }
                    catch { }
                }

                if (p != who) Player.GlobalChat(p, who.color + who.name + Server.DefaultColor + "'s actions for the past &b" + seconds + Server.DefaultColor + " seconds were undone.", false);
                else p.SendMessage("Undid your actions for the past &b" + seconds + Server.DefaultColor + " seconds.");
                return;
            }
            else if (message.Split(' ')[0].ToLower() == "physics")
            {
                if (p.group.Permission < LevelPermission.AdvBuilder) { p.SendMessage("Reserved for Adv+"); return; }
                else if (p.group.Permission == LevelPermission.AdvBuilder && seconds > 1200) { p.SendMessage("AdvBuilders may only undo 1200 seconds."); return; }
                else if (p.group.Permission == LevelPermission.Operator && seconds > 5400) { p.SendMessage("Operators may only undo 5400 seconds."); return; }

                all.Find("pause").Use(p, "120");
                Level.UndoPos uP;

                for (CurrentPos = p.level.currentUndo; CurrentPos != p.level.currentUndo + 1; CurrentPos--)
                {
                    try
                    {
                        if (CurrentPos < 0) CurrentPos = p.level.UndoBuffer.Count - 1;
                        uP = p.level.UndoBuffer[CurrentPos];
                        b = p.level.GetTile(uP.location);
                        if (uP.timePerformed.AddSeconds(seconds) >= DateTime.Now)
                        {
                            if (b == uP.newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            {
                                p.level.IntToPos(uP.location, out ushort x, out ushort y, out ushort z);
                                p.level.Blockchange(p, x, y, z, uP.oldType);
                            }
                        }
                        else break;
                    }
                    catch { }
                }

                all.Find("pause").Use(p, "");
                Player.GlobalMessage("Physics were undone &b" + seconds + Server.DefaultColor + " seconds");
            }
            else
            {
                if (p != null)
                {
                    if (p.group.Permission < LevelPermission.Operator) { p.SendMessage("Reserved for OP+"); return; }
                    if (seconds > 5400 && p.group.Permission == LevelPermission.Operator) { p.SendMessage("Only SuperOPs may undo more than 30 minutes."); return; }
                }

                bool FoundUser = false;

                try
                {
                    for (int i = 0; i < 300; i++)
                    {
                        if (Player.OfflineUndoName[i].ToLower() == message.Split(' ')[0].ToLower())
                        {
                            FoundUser = true;
                            for (CurrentPos = Player.OfflineUndo[i].Count - 1; CurrentPos >= 0; CurrentPos--)
                            {
                                Pos = Player.OfflineUndo[i][CurrentPos];
                                b = Pos.map.GetTile(Pos.x, Pos.y, Pos.z);

                                if (Pos.timePlaced.AddSeconds(seconds) >= DateTime.Now)
                                {
                                    Pos.map.Blockchange(Pos.x, Pos.y, Pos.z, Pos.type, true);
                                    Pos.newtype = Pos.type; Pos.type = b;
                                    p?.RedoBuffer.Add(Pos);
                                }
                                else break;
                            }
                        }
                    }

                    if (FoundUser) Player.GlobalChat(p, Server.FindColor(message.Split(' ')[0]) + message.Split(' ')[0] + Server.DefaultColor + "'s actions for the past &b" + seconds + Server.DefaultColor + " seconds were undone.", false);
                    else p.SendMessage("Could not find player specified.");
                }
                catch { p.SendMessage("Could not find player specified."); }
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/undo [player] [seconds] - Undoes the blockchanges made by [player] in the previous [seconds].");
            p.SendMessage("/undo [player] all - &cWill undo 138 hours for [player] <SuperOP+>");
            p.SendMessage("/undo [player] 0 - &cWill undo 30 minutes <Operator+>");
            p.SendMessage("/undo physics [seconds] - Undoes the physics for the current map");
        }
    }
}