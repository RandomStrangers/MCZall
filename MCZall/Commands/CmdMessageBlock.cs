using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace MCZall
{
    public class CmdMessageBlock : Command
    {
        public override string Name { get { return "mb"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "build"; } }
        public CmdMessageBlock() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            CatchPos cpos;
            cpos.message = "";

            try
            {
                switch (message.Split(' ')[0])
                {
                    case "air": cpos.type = Block.MsgAir; break;
                    case "water": cpos.type = Block.MsgWater; break;
                    case "lava": cpos.type = Block.MsgLava; break;
                    case "black": cpos.type = Block.MsgBlack; break;
                    case "white": cpos.type = Block.MsgWhite; break;
                    case "show": ShowMBs(p); return;
                    case "add":
                        try
                        {
                            p.ClearBlockchange();
                            cpos = (CatchPos)p.blockchangeObject;
                            cpos.message = cpos.message + " " + message.Substring(message.IndexOf(' ') + 1);
                            p.blockchangeObject = cpos;
                            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

                            p.SendMessage("Message appended.");
                        }
                        catch { p.SendMessage("No message stored."); }
                        return;
                    default: cpos.type = Block.MsgWhite; cpos.message = message; break;
                }
            }
            catch { cpos.type = Block.MsgWhite; cpos.message = message; }

            if (cpos.message == "") cpos.message = message.Substring(message.IndexOf(' ') + 1);
            p.blockchangeObject = cpos;

            p.SendMessage("Place where you wish the message block to go or type %5/mb add [message]" + Server.DefaultColor + " to append to this message."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/mb [block] [message] - Places a message in your next block.");
            p.SendMessage("Valid blocks: white, black, air, water, lava");
            p.SendMessage("/mb add [message] appends to end of your current message");
            p.SendMessage("/mb show shows or hides MBs");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            cpos.message = cpos.message.Replace("'", "");

            DataTable Messages = new DataTable("Messages");
            int totalCount = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Messages" + p.level.name + " WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z, Server.mysqlCon))
                { da.Fill(Messages); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }
            Messages.Dispose();

            totalCount = 0;
            MySqlCommand NewMessage;

            if (Messages.Rows.Count == 0)
            {
                NewMessage = Server.mysqlCon.CreateCommand();
                NewMessage.CommandText = "INSERT INTO Messages" + p.level.name + " (X, Y, Z, Message) VALUES (" + (int)x + ", " + (int)y + ", " + (int)z + ", '" + cpos.message + "')";
            retryTag1: try { NewMessage.ExecuteNonQuery(); } catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag1; else Server.ErrorLog(e); }
            }
            else
            {
                NewMessage = Server.mysqlCon.CreateCommand();
                NewMessage.CommandText = "UPDATE Messages" + p.level.name + " SET Message='" + cpos.message + "' WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z;
            retryTag1: try { NewMessage.ExecuteNonQuery(); } catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag1; else Server.ErrorLog(e); }
            }

            p.SendMessage("Message block placed.");
            p.level.Blockchange(x, y, z, cpos.type);
            p.SendBlockchange(x, y, z, cpos.type);

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
            NewMessage.Dispose();
        }

        struct CatchPos { public string message; public byte type; }

        public void ShowMBs(Player p)
        {
            p.showMBs = !p.showMBs;

            DataTable Messages = new DataTable("Messages"); int totalCount = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Messages" + p.level.name, Server.mysqlCon))
                { da.Fill(Messages); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else { Server.ErrorLog(e); return; } }

            int i;

            if (p.showMBs)
            {
                for (i = 0; i < Messages.Rows.Count; i++)
                    p.SendBlockchange((ushort)Messages.Rows[i]["X"], (ushort)Messages.Rows[i]["Y"], (ushort)Messages.Rows[i]["Z"], Block.MsgWhite);
                p.SendMessage("Now showing &a" + i.ToString() + Server.DefaultColor + " MBs.");
            }
            else
            {
                for (i = 0; i < Messages.Rows.Count; i++)
                    p.SendBlockchange((ushort)Messages.Rows[i]["X"], (ushort)Messages.Rows[i]["Y"], (ushort)Messages.Rows[i]["Z"], p.level.GetTile((ushort)Messages.Rows[i]["X"], (ushort)Messages.Rows[i]["Y"], (ushort)Messages.Rows[i]["Z"]));
                p.SendMessage("Now hiding MBs.");
            }
            Messages.Dispose();
        }
    }
}