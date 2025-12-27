using System;

namespace MCZall
{
    public class CmdCmdBind : Command
    {
        public override string Name { get { return "cmdbind"; } }
        public override string Shortcut { get { return "cb"; } }
        public override string Type { get { return "build"; } }
        public CmdCmdBind() { }
        public override void Use(Player p, string message)
        {
            string foundcmd, foundmessage = ""; int foundnum;

            if (message.IndexOf(' ') == -1)
            {
                bool OneFound = false;
                for (int i = 0; i < 10; i++)
                {
                    if (p.cmdBind[i] != null)
                    {
                        p.SendMessage("&c/" + i + Server.DefaultColor + " bound to &b" + p.cmdBind[i] + " " + p.messageBind[i]);
                        OneFound = true;
                    }
                }
                if (!OneFound) p.SendMessage("You have no commands binded");
                return;
            }

            if (message.Split(' ').Length == 1)
            {
                try
                {
                    foundnum = Convert.ToInt16(message);
                    if (p.cmdBind[foundnum] == null) { p.SendMessage("No command stored here yet."); return; }
                    foundcmd = "/" + p.cmdBind[foundnum] + " " + p.messageBind[foundnum];
                    p.SendMessage("Stored command: &b" + foundcmd);
                }
                catch { Help(p); }
            }
            else if (message.Split(' ').Length > 1)
            {
                try
                {
                    foundnum = Convert.ToInt16(message.Split(' ')[message.Split(' ').Length - 1]);
                    foundcmd = message.Split(' ')[0];
                    if (message.Split(' ').Length > 2)
                    {
                        foundmessage = message.Substring(message.IndexOf(' ') + 1);
                        foundmessage = foundmessage.Remove(foundmessage.LastIndexOf(' '));
                    }

                    p.cmdBind[foundnum] = foundcmd;
                    p.messageBind[foundnum] = foundmessage;

                    p.SendMessage("Binded &b/" + foundcmd + " " + foundmessage + " to &c/" + foundnum);
                }
                catch { Help(p); return; }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/cmdbind [command] [num] - Binds [command] to [num]");
            p.SendMessage("[num] must be between 0 and 9");
            p.SendMessage("Use with \"/[num]\" &b(example: /2)");
            p.SendMessage("Use /cmdbind [num] to see stored commands.");
        }
    }
}