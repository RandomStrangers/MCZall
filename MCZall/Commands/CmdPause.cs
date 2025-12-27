using System;

namespace MCZall
{
    class CmdPause : Command
    {
        public override string Name { get { return "pause"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdPause() { }

        public override void Use(Player p, string message)
        {
            if (message == "") message = p.level.name + " 30";
            int foundNum = 0; Level foundLevel;

            if (message.IndexOf(' ') == -1)
            {
                try { foundNum = int.Parse(message); foundLevel = p.level; }
                catch { foundNum = 30; foundLevel = Level.Find(message); }
            }
            else
            {
                try { foundNum = int.Parse(message.Split(' ')[1]); foundLevel = Level.Find(message.Split(' ')[0]); }
                catch { p.SendMessage("Invalid input"); return; }
            }

            if (foundLevel == null) { p.SendMessage("Could not find entered level."); return; }

            try
            {
                if (foundLevel.physPause)
                {
                    foundLevel.physThread.Resume();
                    foundLevel.physResume = DateTime.Now;
                    foundLevel.physPause = false;
                    Player.GlobalMessage("Physics on " + foundLevel.name + " were re-enabled.");
                }
                else
                {
                    foundLevel.physThread.Suspend();
                    foundLevel.physResume = DateTime.Now.AddSeconds(foundNum);
                    foundLevel.physPause = true;
                    Player.GlobalMessage("Physics on " + foundLevel.name + " were temporarily disabled.");

                    foundLevel.physTimer.Elapsed += delegate
                    {
                        if (DateTime.Now > foundLevel.physResume)
                        {
                            foundLevel.physPause = false;
                            foundLevel.physThread.Resume();
                            Player.GlobalMessage("Physics on " + foundLevel.name + " were re-enabled.");
                            foundLevel.physTimer.Stop();
                            foundLevel.physTimer.Dispose();
                        }
                    }; foundLevel.physTimer.Start();
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/pause [map] [amount] - Pauses physics on [map] for 30 seconds");
        }
    }
}
