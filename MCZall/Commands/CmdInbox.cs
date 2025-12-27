using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace MCZall
{
    public class CmdInbox : Command
    {
        public override string Name { get { return "inbox"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdInbox() { }
        public override void Use(Player p, string message)
        {
            try
            {
                MySqlCommand cmdDatabaseCreate = new MySqlCommand("CREATE TABLE if not exists Inbox" + p.name + " (PlayerFrom CHAR(20), TimeSent DATETIME, Contents VARCHAR(255));", Server.mysqlCon);
            retryTag1: try { cmdDatabaseCreate.ExecuteNonQuery(); } catch { goto retryTag1; }
                cmdDatabaseCreate.Dispose();

                if (message == "")
                {
                    DataTable Inbox = new DataTable("Inbox");
                retryTag2: try
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Inbox" + p.name + " ORDER BY TimeSent", Server.mysqlCon))
                        { da.Fill(Inbox); }
                    }
                    catch { goto retryTag2; }

                    if (Inbox.Rows.Count == 0) { p.SendMessage("No messages found."); Inbox.Dispose(); return; }

                    for (int i = 0; i < Inbox.Rows.Count; ++i)
                    {
                        p.SendMessage(i + ": From &5" + Inbox.Rows[i]["PlayerFrom"].ToString() + Server.DefaultColor + " at &a" + Inbox.Rows[i]["TimeSent"].ToString());
                    }
                    Inbox.Dispose();
                }
                else if (message.Split(' ')[0].ToLower() == "del" || message.Split(' ')[0].ToLower() == "delete")
                {
                    int FoundRecord = -1;

                    if (message.Split(' ')[1].ToLower() != "all")
                    {
                        try
                        {
                            FoundRecord = int.Parse(message.Split(' ')[1]);
                        }
                        catch { p.SendMessage("Incorrect number given."); return; }

                        if (FoundRecord < 0) { p.SendMessage("Cannot delete records below 0"); return; }
                    }

                    DataTable Inbox = new DataTable("Inbox");
                retryTag2:
                    try
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Inbox" + p.name + " ORDER BY TimeSent", Server.mysqlCon))
                        { da.Fill(Inbox); }
                    }
                    catch { goto retryTag2; }

                    if (Inbox.Rows.Count - 1 < FoundRecord || Inbox.Rows.Count == 0)
                    {
                        p.SendMessage("\"" + FoundRecord + "\" does not exist."); Inbox.Dispose(); return;
                    }

                    MySqlCommand NewMessage = Server.mysqlCon.CreateCommand();

                    if (FoundRecord == -1)
                        NewMessage.CommandText = "TRUNCATE TABLE Inbox" + p.name;
                    else
                        NewMessage.CommandText = "DELETE FROM Inbox" + p.name + " WHERE PlayerFrom='" + Inbox.Rows[FoundRecord]["PlayerFrom"] + "' AND TimeSent='" + Convert.ToDateTime(Inbox.Rows[FoundRecord]["TimeSent"]).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    retryTag3:
                    try { NewMessage.ExecuteNonQuery(); } catch { goto retryTag3; }

                    if (FoundRecord == -1)
                        p.SendMessage("Deleted all messages.");
                    else
                        p.SendMessage("Deleted message.");

                    Inbox.Dispose();
                    NewMessage.Dispose();
                }
                else
                {
                    int FoundRecord;

                    try
                    {
                        FoundRecord = int.Parse(message);
                    }
                    catch { p.SendMessage("Incorrect number given."); return; }

                    if (FoundRecord < 0) { p.SendMessage("Cannot read records below 0"); return; }

                    DataTable Inbox = new DataTable("Inbox");
                retryTag3:
                    try
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Inbox" + p.name + " ORDER BY TimeSent", Server.mysqlCon))
                        { da.Fill(Inbox); }
                    }
                    catch { goto retryTag3; }

                    if (Inbox.Rows.Count - 1 < FoundRecord || Inbox.Rows.Count == 0)
                    {
                        p.SendMessage("\"" + FoundRecord + "\" does not exist."); Inbox.Dispose(); return;
                    }

                    p.SendMessage("Message from &5" + Inbox.Rows[FoundRecord]["PlayerFrom"] + Server.DefaultColor + " sent at &a" + Inbox.Rows[FoundRecord]["TimeSent"] + ":");
                    p.SendChat(p, Inbox.Rows[FoundRecord]["Contents"].ToString());
                    Inbox.Dispose();
                }
            }
            catch
            {
                p.SendMessage("Error accessing inbox. You may have no mail, try again.");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/inbox - Displays all your messages.");
            p.SendMessage("/inbox [num] - Displays the message at [num]");
            p.SendMessage("/inbox <del> [\"all\"/num] - Deletes the message at Num or All if \"all\" is given.");
        }
    }
}