using MySql.Data.MySqlClient;
using System;

namespace MCZall
{
    public class CmdSend : Command
    {
        public override string Name { get { return "send"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdSend() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            if (p.messageTo == "")
            {
                p.messageTo = message.Split(' ')[0];
                message = message.Remove(0, message.IndexOf(' ') + 1);
            }
            if (message[message.Length - 1].ToString() == ">")
            {
                message = message.Remove(message.Length - 1);
                p.newMessage = p.newMessage + " " + message;
                p.SendMessage("Message to &5" + p.messageTo + Server.DefaultColor + " appended.");
            }
            else
            {
                if (message != "1") if (p.newMessage == "") p.newMessage = message; else p.newMessage = p.newMessage + " " + message;
                Player who = Player.Find(p.messageTo);

                if (who != null) p.messageTo = who.name;

                MySqlCommand cmdDatabaseCreate = new MySqlCommand("CREATE TABLE if not exists Inbox" + p.messageTo + " (PlayerFrom CHAR(20), TimeSent DATETIME, Contents VARCHAR(255));", Server.mysqlCon);
            retryTag: try { cmdDatabaseCreate.ExecuteNonQuery(); } catch { goto retryTag; }
                cmdDatabaseCreate.Dispose();

                //DB
                MySqlCommand NewMessage = Server.mysqlCon.CreateCommand();
                NewMessage.CommandText = "INSERT INTO Inbox" + p.messageTo + " (PlayerFrom, TimeSent, Contents) VALUES ('" + p.name + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + p.newMessage.Replace("'", "") + "')";
            retryTag1: try { NewMessage.ExecuteNonQuery(); } catch { goto retryTag1; }
                NewMessage.Dispose();
                //DB

                p.SendMessage("Message sent to &5" + p.messageTo + Server.DefaultColor + ".");

                who?.SendMessage("Message recieved from &5" + p.name + Server.DefaultColor + ".");

                p.messageTo = ""; p.newMessage = "";
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/send [name] <message> - Sends <message> to [name].");
            p.SendMessage("If \">\" is at the end of <message> then the message will not be sent and allow you to add more by typing /send <message> again.");
        }
    }
}