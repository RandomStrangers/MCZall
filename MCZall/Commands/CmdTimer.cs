namespace MCZall
{
    public class CmdTimer : Command
    {
        public override string Name { get { return "timer"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdTimer() { }
        public override void Use(Player p, string message)
        {
            if (p.cmdTimer == true) { p.SendMessage("Can only have one timer at a time. Use /abort to cancel your previous timer."); return; }

            System.Timers.Timer messageTimer = new System.Timers.Timer(5000);
            if (message == "") { Help(p); return; }

            int TotalTime = 0;
            try
            {
                TotalTime = int.Parse(message.Split(' ')[0]);
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            catch
            {
                TotalTime = 60;
            }

            if (TotalTime > 300) { p.SendMessage("Cannot have more than 5 minutes in a timer"); return; }

            Player.GlobalChatLevel(p, Server.DefaultColor + "Timer lasting for " + TotalTime + " seconds has started.", false);
            TotalTime /= 5;

            Player.GlobalChatLevel(p, Server.DefaultColor + message, false);

            p.cmdTimer = true;
            messageTimer.Elapsed += delegate
            {
                TotalTime--;
                if (TotalTime < 1 || p.cmdTimer == false)
                {
                    p.SendMessage("Timer ended.");
                    messageTimer.Stop();
                }
                else
                {
                    Player.GlobalChatLevel(p, Server.DefaultColor + message, false);
                    p.SendMessage("Timer has " + (TotalTime * 5) + " seconds remaining.");
                }
            };

            messageTimer.Start();
        }
        public override void Help(Player p)
        {
            p.SendMessage("/timer [time] [message] - Starts a timer which repeats [message] every 5 seconds.");
            p.SendMessage("Repeats constantly until [time] has passed");
        }
    }
}