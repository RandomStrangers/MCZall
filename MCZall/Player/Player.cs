/*
	Copyright 2010 MCZall Team Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace MCZall
{
    public sealed class Player
    {
        public static List<Player> players = new List<Player>(64);
        public static Dictionary<string, string> left = new Dictionary<string, string>();
        public static List<Player> connections = new List<Player>(Server.players);
        public static List<string> emoteList = new List<string>();
        public static byte Number { get { return (byte)players.Count; } }
        static readonly System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public MySqlConnection playerCon;

        readonly Socket socket;
        readonly System.Timers.Timer loginTimer = new System.Timers.Timer(1000);
        public System.Timers.Timer pingTimer = new System.Timers.Timer(2000);
        readonly System.Timers.Timer extraTimer = new System.Timers.Timer(12000);
        public System.Timers.Timer afkTimer = new System.Timers.Timer(2000);
        public int afkCount = 0;
        public DateTime afkStart;

        public bool megaBoid = false;
        public bool cmdTimer = false;

        byte[] buffer = new byte[0];
        readonly byte[] tempbuffer = new byte[0xFF];
        public bool disconnected = false;

        public string name;
        public string realName;
        public byte id;
        public int userID = -1;
        public string ip;
        public string color;
        public Group group;
        public bool hidden = false;
        public bool painting = false;
        public bool muted = false;
        public bool jailed = false;
        public bool invincible = false;
        public string prefix = "";

        public bool deleteMode = false;
        public bool ignorePermission = false;
        public bool parseSmiley = true;
        public bool smileySaved = true;
        public bool opchat = false;
        public bool whisper = false;
        public string whisperTo = "";

        public bool trainGrab = false;
        public bool onTrain = false;

        public bool frozen = false;
        public string following = "";

        public int money = 0;
        public long overallBlocks = 0;
        public int loginBlocks = 0;

        public DateTime timeLogged;
        public DateTime firstLogin;
        public int totalLogins = 0;
        public int totalKicked = 0;
        public int overallDeath = 0;

        public bool staticCommands = false;

        public DateTime ZoneSpam;
        public bool ZoneCheck = false;
        public bool zoneDel = false;

        public Thread commThread;
        public bool commUse = false;

        public Thread extraThread;

        //Flying
        public bool isFlying = false;
        public List<FlyPos> FlyBuffer = new List<FlyPos>();
        public List<FlyPos> TempFly = new List<FlyPos>();
        public struct FlyPos { public ushort x, y, z; }
        public bool flyGlass = true;

        //Copy
        public List<CopyPos> CopyBuffer = new List<CopyPos>();
        public struct CopyPos { public ushort x, y, z; public byte type; }
        public bool copyAir = false;

        //Undo
        public struct UndoPos { public ushort x, y, z; public byte type, newtype; public Level map; public DateTime timePlaced; }
        public List<UndoPos> UndoBuffer = new List<UndoPos>();
        public List<UndoPos> RedoBuffer = new List<UndoPos>();


        public static List<UndoPos>[] OfflineUndo = new List<UndoPos>[300];
        public static string[] OfflineUndoName = new string[300];
        public static int LastLogoff = 0;

        public bool showPortals = false;
        public bool showMBs = false;


        //Movement
        public ushort oldBlock = 0;
        public ushort deathCount = 0;
        public byte deathBlock;

        //Games
        public DateTime lastDeath = DateTime.Now;

        //Inbox
        public string messageTo = "";
        public string newMessage = "";

        public byte BlockAction = 0;  //0-Nothing 1-solid 2-lava 3-water 4-active_lava 5 Active_water 6 OpGlass 7 BluePort 8 OrangePort
        public byte modeType = 0;
        public byte[] bindings = new byte[128];
        public string[] cmdBind = new string[10];
        public string[] messageBind = new string[10];
        public string lastCMD = "";

        public Level level = Server.mainLevel;
        public bool Loading = true;     //True if player is loading a map.

        public delegate void BlockchangeEventHandler(Player p, ushort x, ushort y, ushort z, byte type);
        public event BlockchangeEventHandler Blockchange = null;
        public void ClearBlockchange() { Blockchange = null; }
        public bool HasBlockchange() { return Blockchange == null; }
        public object blockchangeObject = null;

        public ushort[] pos = new ushort[3] { 0, 0, 0 };
        ushort[] oldpos = new ushort[3] { 0, 0, 0 };
        ushort[] basepos = new ushort[3] { 0, 0, 0 };
        public byte[] rot = new byte[2] { 0, 0 };
        byte[] oldrot = new byte[2] { 0, 0 };

        // grief/spam detection
        public static int spamBlockCount = 200;
        public static int spamBlockTimer = 5;
        readonly Queue<DateTime> spamBlockLog = new Queue<DateTime>(spamBlockCount);

        public static int spamChatCount = 3;
        public static int spamChatTimer = 4;
        readonly Queue<DateTime> spamChatLog = new Queue<DateTime>(spamChatCount);

        bool loggedIn = false;
        public Player(Socket s)
        {
            try
            {
                socket = s;
                ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                Server.s.Log(ip + " connected.");

                playerCon = new MySqlConnection("Data Source=" + Server.MySQLHost + ";Database=" + Server.MySQLDatabaseName + ";User ID=" + Server.MySQLUsername + ";Password=" + Server.MySQLPassword + ";Pooling=false");
                playerCon.Open();

                if (Server.bannedIP.Contains(ip)) { Kick("You're banned!"); return; }
                if (connections.Count >= 5) { Kick("Too many connections!"); return; }

                for (byte i = 0; i < 128; ++i) bindings[i] = i;

                socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);

                loginTimer.Elapsed += delegate
                {
                    if (!Loading)
                    {
                        loginTimer.Stop();

                        if (File.Exists("text/welcome.txt"))
                        {
                            try
                            {
                                List<string> welcome = new List<string>();
                                StreamReader wm = File.OpenText("text/welcome.txt");
                                while (!wm.EndOfStream)
                                    welcome.Add(wm.ReadLine());

                                wm.Close();
                                wm.Dispose();

                                foreach (string w in welcome)
                                    SendMessage(w);
                            }
                            catch { }
                        }
                        else
                        {
                            Server.s.Log("Could not find Welcome.txt. Using default.");
                            File.WriteAllText("text/welcome.txt", "Welcome to my server!");
                        }
                        extraTimer.Start();
                    }
                }; loginTimer.Start();

                pingTimer.Elapsed += delegate { SendPing(); };
                pingTimer.Start();

                extraTimer.Elapsed += delegate
                {
                    extraTimer.Stop();

                    try
                    {
                        DataTable Inbox = new DataTable("Inbox");
                        using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Inbox" + name, Server.mysqlCon))
                        { da.Fill(Inbox); }

                        SendMessage("You have &a" + Inbox.Rows.Count + Server.DefaultColor + " messages in /inbox");
                        Inbox.Dispose();
                    }
                    catch { }
                    if (Server.updateTimer.Interval > 1000) SendMessage("Lowlag mode is currently &aON.");
                    //SendMessage("You currently have &a" + money + Server.DefaultColor + " moneys");
                    SendMessage("You have modified &a" + overallBlocks + Server.DefaultColor + " blocks!");
                    if (players.Count == 1)
                        SendMessage("There is currently &a" + players.Count + " player online.");
                    else
                        SendMessage("There are currently &a" + players.Count + " players online.");
                };

                afkTimer.Elapsed += delegate
                {
                    if (name == "") return;

                    if (Server.afkset.Contains(name))
                    {
                        afkCount = 0;
                        if (Server.afkkick > 0 && group.Permission < LevelPermission.Operator)
                            if (afkStart.AddMinutes(Server.afkkick) < DateTime.Now)
                                Kick("Auto-kick, AFK for &c" + Server.afkkick + Server.DefaultColor + " minutes");
                        if ((oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2]) && (oldrot[0] != rot[0] || oldrot[1] != rot[1]))
                            Command.all.Find("afk").Use(this, "");
                    }
                    else
                    {
                        if (oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2] && oldrot[0] == rot[0] && oldrot[1] == rot[1])
                            afkCount++;
                        else
                            afkCount = 0;

                        if (afkCount > Server.afkminutes * 30)
                        {
                            Command.all.Find("afk").Use(this, "&cauto:" + Server.DefaultColor + " Not moved for " + Server.afkminutes + " minutes");
                            afkCount = 0;
                        }
                    }
                };
                if (Server.afkminutes > 0) afkTimer.Start();

                connections.Add(this);
            }
            catch (Exception e) { Kick("Login failed!"); Server.ErrorLog(e); }
        }

        public void Save()
        {
            MySqlCommand UpdateUser = playerCon.CreateCommand();
            UpdateUser.CommandText = "UPDATE Players SET IP='" + ip + "', LastLogin='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', totalLogin=" + totalLogins +
                ", Title='" + prefix + "', totalDeaths=" + overallDeath + ", Money=" + money + ", totalBlocks=" + overallBlocks + " + " + loginBlocks +
                ", totalKicked=" + totalKicked + " WHERE Name='" + name + "'";

            int totalTries = 0;
        retryTag: try { UpdateUser.ExecuteNonQuery(); }
            catch (Exception e) { totalTries++; if (totalTries < 10) goto retryTag; else Server.ErrorLog(e); }

            try
            {
                if (!smileySaved)
                {
                    if (parseSmiley)
                    {
                        emoteList.RemoveAll(s => s == name);
                    }
                    else
                    {
                        emoteList.Add(name);
                    }

                    File.WriteAllLines("text/emotelist.txt", emoteList.ToArray());
                    smileySaved = true;
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        #region == INCOMING ==
        static void Receive(IAsyncResult result)
        {
            Player p = (Player)result.AsyncState;
            if (p.disconnected)
                return;
            try
            {
                int length = p.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] b = new byte[p.buffer.Length + length];
                Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
                Buffer.BlockCopy(p.tempbuffer, 0, b, p.buffer.Length, length);

                p.buffer = p.HandleMessage(b);
                p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
                                      new AsyncCallback(Receive), p);
            }
            catch (SocketException)
            {
                p.Disconnect();
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                p.Kick("Error!");
            }
        }
        byte[] HandleMessage(byte[] buffer)
        {
            try
            {
                int length = 0; byte msg = buffer[0];
                // Get the length of the message by checking the first byte
                switch (msg)
                {
                    case 0:
                        length = 130;
                        break; // login
                    case 5:
                        if (!loggedIn)
                            goto default;
                        length = 8;
                        break; // blockchange
                    case 8:
                        if (!loggedIn)
                            goto default;
                        length = 9;
                        break; // input
                    case 13:
                        if (!loggedIn)
                            goto default;
                        length = 65;
                        break; // chat
                    default:
                        Kick("Unhandled message id \"" + msg + "\"!");
                        return new byte[0];
                }
                if (buffer.Length > length)
                {
                    byte[] message = new byte[length];
                    Buffer.BlockCopy(buffer, 1, message, 0, length);

                    byte[] tempbuffer = new byte[buffer.Length - length - 1];
                    Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

                    buffer = tempbuffer;

                    // Thread thread = null; 
                    switch (msg)
                    {
                        case 0:
                            HandleLogin(message);
                            break;
                        case 5:
                            if (!loggedIn)
                                break;
                            HandleBlockchange(message);
                            break;
                        case 8:
                            if (!loggedIn)
                                break;
                            HandleInput(message);
                            break;
                        case 13:
                            if (!loggedIn)
                                break;
                            HandleChat(message);
                            break;
                    }
                    //thread.Start((object)message);
                    if (buffer.Length > 0)
                        buffer = HandleMessage(buffer);
                    else
                        return new byte[0];
                }
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
            }
            return buffer;
        }
        void HandleLogin(byte[] message)
        {
            try
            {
                //byte[] message = (byte[])m;
                if (loggedIn)
                    return;

                byte version = message[0];
                name = enc.GetString(message, 1, 64).Trim();
                string verify = enc.GetString(message, 65, 32).Trim();
                byte type = message[129];

                if (Server.banned.Contains(name)) { Kick("You're banned!"); return; }
                if (players.Count >= Server.players) { Kick("Server full!"); return; }
                if (version != Server.version) { Kick("Wrong version!"); return; }
                if (name.Length > 16 || !ValidName(name)) { Kick("Illegal name!"); return; }

                if (Server.verify)
                {
                    if (verify == "--" || verify != BitConverter.ToString(
                        md5.ComputeHash(enc.GetBytes(Server.salt + name))).
                        Replace("-", "").ToLower().TrimStart('0'))
                    {
                        if (ip != "127.0.0.1" && !ip.Contains("192.168."))
                        {
                            Kick("Login failed! Try again."); return;
                        }
                    }
                }

                foreach (Player p in players)
                {
                    if (p.name == name)
                    {
                        if (Server.verify)
                        {
                            p.Kick("Someone logged in as you!"); break;
                        }
                        else { Kick("Already logged in!"); return; }
                    }
                }

                Server.s.Log(ip + " logging in as " + name + ".");

                try { left.Remove(name.ToLower()); } catch { }
                SendMotd();
                SendMap(); Loading = true;

                if (disconnected) return;

                loggedIn = true;
                id = FreeId();

                if (Server.operators.Contains(name))
                    group = Group.Find("operator");
                else if (Server.builders.Contains(name))
                    group = Group.Find("builder");
                else if (Server.superOps.Contains(name))
                    group = Group.Find("superop");
                else if (Server.bot.Contains(name))
                    group = Group.Find("bots");
                else if (Server.advbuilders.Contains(name))
                    group = Group.Find("advbuilder");
                else
                    group = Group.standard;

                players.Add(this);
                connections.Remove(this);

                Server.s.PlayerListUpdate();

                color = group.Color;

                /*
				if (!Server.console && Server.win != null)
					Server.win.UpdateClientList(players);
				*/
                IRCBot.Say(name + " joined the game.");

                //Test code to show wehn people come back with different accounts on the same IP
                string temp = "Lately known as:";
                bool found = false;
                if (ip != "127.0.0.1")
                {
                    foreach (KeyValuePair<string, string> prev in left)
                    {
                        if (prev.Value == ip)
                        {
                            found = true;
                            temp += " " + prev.Key;
                        }
                    }
                    if (found)
                    {
                        GlobalMessageOps(temp);
                        Server.s.Log(temp);
                        IRCBot.Say(temp);       //Tells people in IRC only hopefully
                    }
                }

                ushort x = (ushort)((0.5 + level.spawnx) * 32);
                ushort y = (ushort)((1 + level.spawny) * 32);
                ushort z = (ushort)((0.5 + level.spawnz) * 32);
                pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };

                GlobalSpawn(this, x, y, z, rot[0], rot[1], true);
                foreach (Player p in players)
                {
                    if (p.level == level && p != this && !p.hidden)
                        SendSpawn(p.id, p.color + p.name, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                }
                foreach (PlayerBot pB in PlayerBot.playerbots)
                {
                    if (pB.level == level)
                        SendSpawn(pB.id, pB.color + pB.name, pB.pos[0], pB.pos[1], pB.pos[2], pB.rot[0], pB.rot[1]);
                }
                Loading = false;
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                GlobalMessage("An error occurred: " + e.Message);
            }

            DataTable playerDb = new DataTable("playerDb");
            int totalTries = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Players WHERE Name='" + name + "'", Server.mysqlCon))
                { da.Fill(playerDb); }
            }
            catch (Exception e) { totalTries++; if (totalTries < 10) goto retryTag; else Server.ErrorLog(e); }

            if (name == "Zallist")
            {
                color = "&e";
                prefix = "[Dev] ";
            }

            if (playerDb.Rows.Count == 0)
            {
                SendMessage("Welcome " + name + "! This is your first visit.");
                firstLogin = DateTime.Now;
                totalLogins = 1;
                timeLogged = DateTime.Now;


                MySqlCommand UpdateUser = playerCon.CreateCommand();
                UpdateUser.CommandText = "INSERT INTO Players (Name, IP, FirstLogin, LastLogin, totalLogin, Title, totalDeaths, Money, totalBlocks, totalKicked)" +
                    "VALUES ('" + name + "', '" + ip + "', '" + firstLogin.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " + totalLogins +
                    ", '" + prefix + "', " + overallDeath + ", " + money + ", " + loginBlocks + ", " + totalKicked + ")";

                totalTries = 0;
            retryTag1: try { UpdateUser.ExecuteNonQuery(); } catch (Exception e) { totalTries++; if (totalTries < 10) goto retryTag1; else Server.ErrorLog(e); }

            }
            else
            {
                totalLogins = int.Parse(playerDb.Rows[0]["totalLogin"].ToString()) + 1;
                userID = int.Parse(playerDb.Rows[0]["ID"].ToString());
                firstLogin = DateTime.Parse(playerDb.Rows[0]["firstLogin"].ToString());
                timeLogged = DateTime.Now;
                if (playerDb.Rows[0]["Title"].ToString().Trim() != "")
                    if (playerDb.Rows[0]["Title"].ToString().Trim()[0].ToString() == "[") prefix = playerDb.Rows[0]["Title"].ToString().Trim() + " ";
                    else prefix = "[" + playerDb.Rows[0]["Title"].ToString().Trim() + "] ";
                overallDeath = int.Parse(playerDb.Rows[0]["TotalDeaths"].ToString());
                overallBlocks = int.Parse(playerDb.Rows[0]["totalBlocks"].ToString().Trim());
                money = int.Parse(playerDb.Rows[0]["Money"].ToString());
                totalKicked = int.Parse(playerDb.Rows[0]["totalKicked"].ToString());
                SendMessage("Welcome back " + prefix + name + "! You've been here " + totalLogins + " times!");
            }
            playerDb.Dispose();

            if (emoteList.Contains(name)) parseSmiley = false;

            GlobalChat(null, "&a+ " + color + prefix + name + Server.DefaultColor + " has joined the game.", false);
        }

        void HandleBlockchange(byte[] message)
        {
            int section = 0;
            try
            {
                if (group.Name == "bots") { return; } //connected bots cant do block changes
                //byte[] message = (byte[])m;
                if (!loggedIn)
                    return;
                if (CheckBlockSpam())
                    return;

                section++;
                ushort x = NTHO(message, 0);
                ushort y = NTHO(message, 2);
                ushort z = NTHO(message, 4);
                byte action = message[6];
                byte type = message[7];

                section++;
                if (type > 49)
                {
                    Kick("Unknown block type!");
                    return;
                }
                section++;
                byte b = level.GetTile(x, y, z);
                if (b == Block.Zero) { return; }
                if (jailed) { SendBlockchange(x, y, z, b); return; }
                if (level.name.Contains("Museum " + Server.DefaultColor) && Blockchange == null)
                {
                    SendMessage("Cannot modify a museum.");
                    SendBlockchange(x, y, z, b);
                    return;
                }

                if (!deleteMode)
                {
                    string info = level.FoundInfo(x, y, z);
                    if (info.Contains("wait")) { return; }
                }

                section++;
                if (Blockchange != null)
                {
                    if (Blockchange.Method.ToString().IndexOf("AboutBlockchange") == -1)
                    {
                        MySqlCommand InsertBlockDelete = playerCon.CreateCommand();

                        InsertBlockDelete.CommandText = "INSERT INTO Block" + level.name + " (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('" + name + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " + (int)x + ", " + (int)y + ", " + (int)z + ", " + type + ", true)";

                        int totalCount = 0;

                    retryTag: try
                        {
                            InsertBlockDelete.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            totalCount++;
                            if (totalCount < 10) goto retryTag;
                            else Server.ErrorLog(e);
                        }
                        InsertBlockDelete.Dispose();
                    }

                    Blockchange(this, x, y, z, type);
                    return;
                }

                section++;
                if (group.Permission == LevelPermission.Guest)
                {
                    if (group.Name == "banned") return;

                    int Diff = 0;

                    Diff = Math.Abs(pos[0] / 32 - x);
                    Diff += Math.Abs(pos[1] / 32 - y);
                    Diff += Math.Abs(pos[2] / 32 - z);

                    if (Diff > 12)
                    {
                        Server.s.Log(name + " attempted to build with a " + Diff.ToString() + " distance offset");
                        GlobalMessageOps("To Ops &f-" + color + name + "&f- attempted to build with a " + Diff.ToString() + " distance offset");
                        SendMessage("You can't build that far away.");
                        SendBlockchange(x, y, z, b); return;
                    }

                    if (Server.antiTunnel)
                    {
                        if (y < level.depth / 2 - Server.maxDepth)
                        {
                            SendMessage("You're not allowed to build this far down!");
                            SendBlockchange(x, y, z, b); return;
                        }
                    }
                }
                section++;
                if (b == 7)
                {
                    if (CheckOp())
                    {
                        Server.s.Log(name + " attempted to delete an adminium block.");
                        GlobalMessageOps("To Ops &f-" + color + name + "&f- attempted to delete an adminium block.");
                        Kick("Tried to delete adminium (epic timing)");
                        return;
                    }
                }
                section++;

                LevelPermission foundRank = Block.AllowPlace(b);
                if (foundRank > group.Permission && !Block.BuildIn(b) && !Block.AllowBreak(b))
                {
                    SendMessage("Block rank: &b" + Level.PermissionToName(foundRank));
                    SendBlockchange(x, y, z, b);
                    return;
                }

                if (Block.AllowPlace(type) > group.Permission)
                {
                    SendMessage("You can't place this block type!");
                    SendBlockchange(x, y, z, b); return;
                }

                if (b >= 200 && b < 220)
                {
                    SendMessage("Block is active, you cant disturb it!");
                    SendBlockchange(x, y, z, b);
                    return;
                }

                section++;

                if (action > 1) { Kick("Unknown block action!"); }
                type = bindings[type];
                section++;
                //Ignores updating blocks that are the same and send block only to the player
                if (b == (byte)((painting || action == 1) ? type : 0))
                {
                    if (painting || message[7] != type) { SendBlockchange(x, y, z, b); }
                    return;
                }
                section++;
                //else

                if (!painting && action == 0)
                {
                    if (!deleteMode)
                    {
                        if (Block.Portal(b)) { HandlePortal(this, x, y, z, b); return; }
                        if (Block.Mb(b)) { HandleMsgBlock(this, x, y, z, b); return; }
                    }

                    //DB
                    MySqlCommand InsertBlockDelete = playerCon.CreateCommand();
                    InsertBlockDelete.CommandText = "INSERT INTO Block" + level.name + " (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('" + name + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " + (int)x + ", " + (int)y + ", " + (int)z + ", " + type + ", true)";

                    int totalCount = 0;

                retryTag: try
                    {
                        InsertBlockDelete.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        totalCount++;
                        if (totalCount < 50) goto retryTag;
                        else Server.ErrorLog(e);
                    }
                    InsertBlockDelete.Dispose();

                    DeleteBlock(b, x, y, z);
                }
                else
                {
                    //DB
                    MySqlCommand InsertBlockDelete = playerCon.CreateCommand();
                    InsertBlockDelete.CommandText = "INSERT INTO Block" + level.name + " (Username, TimePerformed, X, Y, Z, type, deleted) VALUES ('" + name + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " + (int)x + ", " + (int)y + ", " + (int)z + ", " + type + ", false)";

                    int totalCount = 0;

                retryTag: try
                    {
                        InsertBlockDelete.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        totalCount++;
                        if (totalCount < 50) goto retryTag;
                        else Server.ErrorLog(e);
                    }
                    InsertBlockDelete.Dispose();

                    PlaceBlock(b, type, x, y, z);
                }
                section++;

            }
            catch (Exception e)
            {
                // Don't ya just love it when the server tattles?
                Server.ErrorLog(name + " has triggered a block change error");
                GlobalMessageOps(name + " has triggered a block change error");
                IRCBot.Say(name + " has triggered a block change error");
                Server.ErrorLog(e); GlobalMessage("An error occurred in section " + section + " : " + e.Message);
            }
        }

        public void HandlePortal(Player p, ushort x, ushort y, ushort z, byte b)
        {
            try
            {
                DataTable Portals = new DataTable("Portals");

                int totalCount = 0;
            retryTag: try
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Portals" + level.name + " WHERE EntryX=" + (int)x + " AND EntryY=" + (int)y + " AND EntryZ=" + (int)z, Server.mysqlCon))
                    { da.Fill(Portals); }
                }
                catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }

                int LastPortal = Portals.Rows.Count - 1;
                if (LastPortal > -1)
                {
                    if (level.name != Portals.Rows[LastPortal]["ExitMap"].ToString())
                    {
                        ignorePermission = true;
                        Command.all.Find("goto").Use(this, Portals.Rows[LastPortal]["ExitMap"].ToString());
                        ignorePermission = false;
                    }
                    else SendBlockchange(x, y, z, b);

                    Command.all.Find("move").Use(this, Portals.Rows[LastPortal]["ExitX"].ToString() + " " + Portals.Rows[LastPortal]["ExitY"].ToString() + " " + Portals.Rows[LastPortal]["ExitZ"].ToString());
                }
                else
                {
                    Blockchange(this, x, y, z, 0);
                }
                Portals.Dispose();
            }
            catch { p.SendMessage("Portal had no exit."); return; }
        }

        public void HandleMsgBlock(Player p, ushort x, ushort y, ushort z, byte b)
        {
            try
            {
                DataTable Messages = new DataTable("Messages");
                int totalCount = 0;

            retryTag: try
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Messages" + level.name + " WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z, Server.mysqlCon))
                    { da.Fill(Messages); }
                }
                catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }

                int LastMsg = Messages.Rows.Count - 1;
                if (LastMsg > -1)
                {
                    p.SendMessage(Messages.Rows[LastMsg]["Message"].ToString().Trim());
                    SendBlockchange(x, y, z, b);
                }
                else
                {
                    Blockchange(this, x, y, z, 0);
                }
                Messages.Dispose();
            }
            catch { p.SendMessage("No message was stored."); return; }
        }

        private bool CheckOp()
        {
            return group.Name != "operator" && group.Name != "superop";
        }

        private void DeleteBlock(byte b, ushort x, ushort y, ushort z)
        {
            Random rand = new Random();
            int mx, mz;

            if (deleteMode) { level.Blockchange(this, x, y, z, Block.air); return; }

            if (Block.TDoor(b)) { SendBlockchange(x, y, z, b); return; }
            if (Block.DoorAirs(b) != 0)
            {
                if (level.physics != 0) level.Blockchange(x, y, z, Block.DoorAirs(b));
                else SendBlockchange(x, y, z, b);
                return;
            }
            if (Block.Odoor(b) != Block.Zero)
            {
                if (b == Block.odoor8 || b == Block.odoor8_air)
                {
                    level.Blockchange(this, x, y, z, Block.Odoor(b));
                }
                else
                {
                    SendBlockchange(x, y, z, b);
                }
                return;
            }

            switch (b)
            {
                case Block.door_air:   //Door_air
                case Block.door2_air:
                case Block.door3_air:
                case Block.door4_air:
                case Block.door5_air:
                case Block.door6_air:
                case Block.door7_air:
                case Block.door8_air:
                case Block.door9_air:
                case Block.door10_air:
                    break;
                case Block.rocketstart:
                    if (level.physics < 2)
                    {
                        SendBlockchange(x, y, z, b);
                    }
                    else
                    {
                        int newZ = 0, newX = 0, newY = 0;

                        SendBlockchange(x, y, z, Block.rocketstart);
                        if (rot[0] < 48 || rot[0] > (256 - 48))
                            newZ = -1;
                        else if (rot[0] > (128 - 48) && rot[0] < (128 + 48))
                            newZ = 1;

                        if (rot[0] > (64 - 48) && rot[0] < (64 + 48))
                            newX = 1;
                        else if (rot[0] > (192 - 48) && rot[0] < (192 + 48))
                            newX = -1;

                        if (rot[1] >= 192 && rot[1] <= (192 + 32))
                            newY = 1;
                        else if (rot[1] <= 64 && rot[1] >= 32)
                            newY = -1;

                        if (192 <= rot[1] && rot[1] <= 196 || 60 <= rot[1] && rot[1] <= 64) { newX = 0; newZ = 0; }

                        level.Blockchange((ushort)(x + newX * 2), (ushort)(y + newY * 2), (ushort)(z + newZ * 2), Block.rockethead);
                        level.Blockchange((ushort)(x + newX), (ushort)(y + newY), (ushort)(z + newZ), Block.fire);
                    }
                    break;
                case Block.firework:
                    if (level.physics != 0)
                    {
                        mx = rand.Next(0, 2); mz = rand.Next(0, 2);

                        level.Blockchange((ushort)(x + mx - 1), (ushort)(y + 2), (ushort)(z + mz - 1), Block.firework);
                        level.Blockchange((ushort)(x + mx - 1), (ushort)(y + 1), (ushort)(z + mz - 1), Block.lavastill, false, "wait 1 dissipate 100");
                    }
                    SendBlockchange(x, y, z, b);

                    break;
                default:
                    level.Blockchange(this, x, y, z, Block.air);
                    break;
            }
        }

        public void PlaceBlock(byte b, byte type, ushort x, ushort y, ushort z)
        {
            if (Block.Odoor(b) != Block.Zero) { SendMessage("oDoor here!"); return; }

            switch (BlockAction)
            {
                case 0:     //normal
                    if (level.physics == 0)
                    {
                        switch (type)
                        {
                            case Block.dirt: //instant dirt to grass
                                level.Blockchange(this, x, y, z, Block.grass);
                                break;
                            case Block.staircasestep:    //stair handler
                                if (level.GetTile(x, (ushort)(y - 1), z) == Block.staircasestep)
                                {
                                    SendBlockchange(x, y, z, Block.air);    //send the air block back only to the user.
                                    //level.Blockchange(this, x, y, z, (byte)(Block.air));
                                    level.Blockchange(this, x, (ushort)(y - 1), z, Block.staircasefull);
                                    break;
                                }
                                //else
                                level.Blockchange(this, x, y, z, type);
                                break;
                            default:
                                level.Blockchange(this, x, y, z, type);
                                break;
                        }
                    }
                    else
                    {
                        level.Blockchange(this, x, y, z, type);
                    }
                    break;
                case 6:
                    if (b == modeType) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, modeType);
                    break;
                case 13:    //Small TNT
                    level.Blockchange(this, x, y, z, Block.smalltnt);
                    break;
                case 14:    //Small TNT
                    level.Blockchange(this, x, y, z, Block.bigtnt);
                    break;
                default:
                    Server.s.Log(name + " is breaking something");
                    BlockAction = 0;
                    break;
            }
        }

        void HandleInput(object m)
        {
            byte[] message = (byte[])m;
            if (!loggedIn)
                return;

            if (frozen)
            {
                unchecked { SendPos((byte)-1, pos[0], pos[1], pos[2], rot[0], rot[1]); }
                return;
            }
            else if (following != "")
            {
                Player who = Find(following); if (who == null) { following = ""; return; }
                unchecked { SendPos((byte)-1, who.pos[0], (ushort)(who.pos[1] - 16), who.pos[2], who.rot[0], who.rot[1]); }
            }
            else if (trainGrab) return;

            ushort x = NTHO(message, 1);
            ushort y = NTHO(message, 3);
            ushort z = NTHO(message, 5);
            byte rotx = message[7];
            byte roty = message[8];
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };

            x = (ushort)(x / 32);
            y = (ushort)(y / 32);
            z = (ushort)(z / 32);

            if (level.Death) RealDeath(x, y, z);
            CheckBlock(x, y, z);

            oldBlock = (ushort)(x + y + z);
        }

        public void RealDeath(ushort x, ushort y, ushort z)
        {
            byte b = level.GetTile(x, (ushort)(y - 2), z);
            byte b1 = level.GetTile(x, y, z);

            if (oldBlock != (ushort)(x + y + z))
            {
                if (Block.Convert(b) == Block.air)
                {
                    deathCount++;
                    deathBlock = Block.air;
                    return;
                }
                else
                {
                    if (deathCount > level.fall && deathBlock == Block.air)
                    {
                        HandleDeath(deathBlock);
                        deathCount = 0;
                    }
                    else if (deathBlock != Block.water)
                    {
                        deathCount = 0;
                    }
                }
            }

            switch (Block.Convert(b1))
            {
                case Block.water:
                case Block.waterstill:
                case Block.lava:
                case Block.lavastill:
                    deathCount++;
                    deathBlock = Block.water;
                    if (deathCount > level.drown)
                    {
                        HandleDeath(deathBlock);
                        deathCount = 0;
                    }
                    break;
                default:
                    deathCount = 0;
                    break;
            }
        }

        public void CheckBlock(ushort x, ushort y, ushort z)
        {
            y = (ushort)Math.Round((decimal)(((y * 32) + 4) / 32));

            byte b = level.GetTile(x, y, z);
            byte b1 = level.GetTile(x, (ushort)(y - 1), z);

            if (Block.Mover(b) || Block.Mover(b1))
            {
                if (Block.DoorAirs(b) != 0)
                    level.Blockchange(x, y, z, Block.DoorAirs(b));
                if (Block.DoorAirs(b1) != 0)
                    level.Blockchange(x, (ushort)(y - 1), z, Block.DoorAirs(b1));

                if ((x + y + z) != oldBlock)
                {
                    if (b == Block.air_portal || b == Block.water_portal || b == Block.lava_portal)
                    {
                        HandlePortal(this, x, y, z, b);
                    }
                    else if (b1 == Block.air_portal || b1 == Block.water_portal || b1 == Block.lava_portal)
                    {
                        HandlePortal(this, x, (ushort)(y - 1), z, b1);
                    }

                    if (b == Block.MsgAir || b == Block.MsgWater || b == Block.MsgLava)
                    {
                        HandleMsgBlock(this, x, y, z, b);
                    }
                    else if (b1 == Block.MsgAir || b1 == Block.MsgWater || b1 == Block.MsgLava)
                    {
                        HandleMsgBlock(this, x, (ushort)(y - 1), z, b1);
                    }
                }
            }

            if (isFlying && (x + y + z) != oldBlock)
                if (x > 1 && y > 3 && z > 1 && x < level.width && y < level.height && z < level.width)
                    HandleFly(this, x, (ushort)(y - 2), z);

            if (Block.Death(b)) HandleDeath(b, x, y, z); else if (Block.Death(b1)) HandleDeath(b1, x, y, z);
        }

        public void HandleDeath(byte b, ushort x = 0, ushort y = 0, ushort z = 0)
        {
            if (lastDeath.AddSeconds(2) < DateTime.Now)
            {
                if (level.Killer && !invincible)
                {
                    switch (b)
                    {
                        case Block.tntexplosion: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " &cblew into pieces.", false); break;
                        case Block.deathair: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " walked into &cnerve gas and suffocated.", false); break;
                        case Block.deathwater:
                        case Block.activedeathwater: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " stepped in &dcold water and froze.", false); break;
                        case Block.deathlava:
                        case Block.activedeathlava: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " stood in &cmagma and melted.", false); break;
                        case Block.magma: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was hit by &cflowing magma and melted.", false); break;
                        case Block.geyser: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was hit by &cboiling water and melted.", false); break;
                        case Block.birdkill: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was hit by a &cphoenix and burnt.", false); break;
                        case Block.train: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was hit by a &ctrain.", false); break;
                        case Block.fishshark: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was eaten by a &cshark.", false); break;
                        case Block.fire: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " burnt to a &ccrisp.", false); break;
                        case Block.rockethead: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was &cin a fiery explosion.", false); level.MakeExplosion(x, y, z, 0); break;
                        case Block.zombiebody: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " died due to lack of &5brain.", false); break;
                        case Block.creeper: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was killed &cb-SSSSSSSSSSSSSS", false); level.MakeExplosion(x, y, z, 1); break;
                        case Block.air: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " hit the floor &chard.", false); break;
                        case Block.water: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " &cdrowned.", false); break;
                        case Block.Zero: GlobalChatLevel(this, color + prefix + name + Server.DefaultColor + " was &cterminated", false); break;
                    }

                    Command.all.Find("spawn").Use(this, ""); overallDeath++;

                    if (Server.deathcount)
                        if (overallDeath % 10 == 0) GlobalChat(this, color + prefix + name + Server.DefaultColor + " has died &3" + overallDeath + " times", false);
                }
                lastDeath = DateTime.Now;
            }
        }

        void HandleFly(Player p, ushort x, ushort y, ushort z)
        {
            FlyPos pos;

            ushort xx; ushort yy; ushort zz;

            TempFly.Clear();

            if (!flyGlass) y = (ushort)(y + 1);

            for (yy = y; yy >= (ushort)(y - 1); --yy)
                for (xx = (ushort)(x - 2); xx <= (ushort)(x + 2); ++xx)
                    for (zz = (ushort)(z - 2); zz <= (ushort)(z + 2); ++zz)
                        if (p.level.GetTile(xx, yy, zz) == Block.air)
                        {
                            pos.x = xx; pos.y = yy; pos.z = zz;
                            TempFly.Add(pos);
                        }

            FlyBuffer.ForEach(delegate (FlyPos pos2)
            {
                try { if (!TempFly.Contains(pos2)) SendBlockchange(pos2.x, pos2.y, pos2.z, Block.air); } catch { }
            });

            FlyBuffer.Clear();

            TempFly.ForEach(delegate (FlyPos pos3)
            {
                FlyBuffer.Add(pos3);
            });

            if (flyGlass)
            {
                FlyBuffer.ForEach(delegate (FlyPos pos1)
                {
                    try { SendBlockchange(pos1.x, pos1.y, pos1.z, Block.glass); } catch { }
                });
            }
            else
            {
                FlyBuffer.ForEach(delegate (FlyPos pos1)
                {
                    try { SendBlockchange(pos1.x, pos1.y, pos1.z, Block.waterstill); } catch { }
                });
            }
        }

        void HandleChat(byte[] message)
        {
            try
            {
                if (!loggedIn) return;
                if (!group.CanChat) return;

                //byte[] message = (byte[])m;
                string text = enc.GetString(message, 1, 64).Trim();

                text = Regex.Replace(text, @"\s\s+", " ");
                foreach (char ch in text)
                {
                    if (ch < 32 || ch >= 127 || ch == '&')
                    {
                        Kick("Illegal character in chat message!");
                        return;
                    }
                }
                if (text.Length == 0)
                    return;
                afkCount = 0;
                if (Server.afkset.Contains(name))
                    Command.all.Find("afk").Use(this, "");

                if (text[0] == '/' || text[0] == '!')
                {
                    text = text.Remove(0, 1);

                    int pos = text.IndexOf(' ');
                    if (pos == -1)
                    {
                        HandleCommand(text.ToLower(), "");
                        return;
                    }
                    string cmd = text.Substring(0, pos).ToLower();
                    string msg = text.Substring(pos + 1);
                    HandleCommand(cmd, msg);
                    return;
                }

                if (muted) { SendMessage("You are muted."); return; }  //Muted: Only allow commands

                if (text[0] == '@' || whisper)
                {
                    string newtext = text;
                    if (text[0] == '@') newtext = text.Remove(0, 1).Trim();

                    if (whisperTo == "")
                    {
                        int pos = newtext.IndexOf(' ');
                        if (pos != -1)
                        {
                            string to = newtext.Substring(0, pos);
                            string msg = newtext.Substring(pos + 1);
                            HandleQuery(to, msg); return;
                        }
                        else
                        {
                            SendMessage("No message entered");
                            return;
                        }
                    }
                    else
                    {
                        HandleQuery(whisperTo, newtext);
                        return;
                    }
                }
                if (text[0] == '#' || opchat)
                {
                    string newtext = text;
                    if (text[0] == '#') newtext = text.Remove(0, 1).Trim();

                    GlobalMessageOps("To Ops &f-" + color + name + "&f- " + newtext);
                    if (group.Name != "operator" && group.Name != "superop")
                        SendMessage("To Ops &f-" + color + name + "&f- " + newtext);
                    Server.s.Log("(OPs): " + name + ": " + newtext);
                    return;
                }

                if (!level.worldChat)
                {
                    Server.s.Log("<" + name + ">[level] " + text);
                    GlobalChatLevel(this, text, true);
                    return;
                }

                if (text[0] == '%')
                {
                    string newtext = text;
                    if (!Server.worldChat)
                    {
                        newtext = text.Remove(0, 1).Trim();
                        GlobalChatWorld(this, newtext, true);
                    }
                    else
                    {
                        GlobalChat(this, newtext);
                    }
                    Server.s.Log("<" + name + "> " + newtext);
                    IRCBot.Say("<" + name + "> " + newtext);
                    return;
                }
                Server.s.Log("<" + name + "> " + text);

                if (Server.worldChat)
                {
                    GlobalChat(this, text);
                }
                else
                {
                    GlobalChatLevel(this, text, true);
                }

                IRCBot.Say(name + ": " + text);
            }
            catch (Exception e) { Server.ErrorLog(e); GlobalMessage("An error occurred: " + e.Message); }
        }
        public void HandleCommand(string cmd, string message)
        {
            try
            {
                if (cmd == "") { SendMessage("No command entered."); return; }
                if (jailed) { SendMessage("You cannot use any commands while jailed."); return; }
                if (cmd.ToLower() == "care") { SendMessage("Corneria now loves you with all his heart."); return; }
                if (cmd.ToLower() == "facepalm") { SendMessage("Lawlcat's bot army just simultaneously facepalm'd at your use of this command."); return; }

                if (cmd.Length == 1)
                    try
                    {
                        message = messageBind[Convert.ToInt16(cmd)] + " " + message;
                        message = message.TrimEnd(' ');
                        cmd = cmdBind[Convert.ToInt16(cmd)];
                    }
                    catch { }

                string foundShortcut = Command.all.FindShort(cmd);
                if (foundShortcut != "") cmd = foundShortcut;

                Command command = Command.all.Find(cmd);
                if (command != null)
                {
                    if (group.CanExecute(command))
                    {
                        if (cmd != "repeat") lastCMD = cmd + " " + message;
                        if (level.name.Contains("Museum " + Server.DefaultColor))
                        {
                            switch (cmd.ToLower())
                            {
                                case "abort":
                                case "admins":
                                case "afk":
                                case "ban":
                                case "banip":
                                case "banned":
                                case "clones":
                                case "color":
                                case "copy":
                                case "deletelvl":
                                case "fly":
                                case "follow":
                                case "freeze":
                                case "goto":
                                case "help":
                                case "hide":
                                case "host":
                                case "inbox":
                                case "info":
                                case "invincible":
                                case "kick":
                                case "kickban":
                                case "levels":
                                case "limit":
                                case "load":
                                case "lowlag":
                                case "mapinfo":
                                case "me":
                                case "move":
                                case "museum":
                                case "mute":
                                case "newlvl":
                                case "ops":
                                case "players":
                                case "redo":
                                case "resetbot":
                                case "reveal":
                                case "roll":
                                case "rules":
                                case "say":
                                case "send":
                                case "serverreport":
                                case "setrank":
                                case "spawn":
                                case "spin":
                                case "time":
                                case "timer":
                                case "title":
                                case "tp":
                                case "unban":
                                case "unbanip":
                                case "undo":
                                case "unload":
                                case "unloaded":
                                case "whois":
                                case "whowas": break;

                                default: SendMessage("Cannot use this command while in a museum"); return;
                            }
                        }

                        Server.s.CommandUsed(name + " used /" + cmd + " " + message);
                        commThread = new Thread(new ThreadStart(delegate
                        {
                            command.Use(this, message);
                            commThread.Abort();
                        }));
                        commThread.Start();
                    }
                    else { SendMessage("You are not allowed to use \"" + cmd + "\"!"); }
                }
                else if (Block.Byte(cmd.ToLower()) != Block.Zero)
                {
                    HandleCommand("mode", cmd.ToLower());
                }
                else { SendMessage("Unknown command \"" + cmd + "\"!"); }
            }
            catch (Exception e) { Server.s.Log(e.Message); SendMessage("Command failed."); }
        }
        void HandleQuery(string to, string message)
        {
            Player p = Find(to);
            if (p == this) { SendMessage("Trying to talk to yourself, huh?"); return; }
            if (p != null && !p.hidden)
            {
                Server.s.Log(name + " @" + p.name + ": " + message);
                SendChat(this, Server.DefaultColor + "[<] " + p.color + p.prefix + p.name + ": &f" + message);
                SendChat(p, "&9[>] " + color + prefix + name + ": &f" + message);
            }
            else { SendMessage("Player \"" + to + "\" doesn't exist!"); }
        }
        #endregion
        #region == OUTGOING ==
        public void SendRaw(int id)
        {
            SendRaw(id, new byte[0]);
        }
        public void SendRaw(int id, byte[] send)
        {
            byte[] buffer = new byte[send.Length + 1];
            buffer[0] = (byte)id;

            Buffer.BlockCopy(send, 0, buffer, 1, send.Length);

            int tries = 0;

        retry: try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate (IAsyncResult result) { }, null);
            }
            catch (SocketException)
            {
                tries++;
                if (tries > 2)
                    Disconnect();
                else goto retry;
            }
        }
        public void SendMessage(string message)
        {
            if (this == null) { Server.s.Log(message); return; }
            unchecked { SendMessage(id, Server.DefaultColor + message); }
        }
        public void SendChat(Player p, string message)
        {
            if (this == null) { Server.s.Log(message); return; }
            p.SendMessage(p.id, message);
        }
        public void SendMessage(byte id, string message)
        {
            if (this == null) { Server.s.Log(message); return; }
            if (ZoneSpam.AddSeconds(2) > DateTime.Now && message.Contains("This zone belongs to ")) return;

            byte[] buffer = new byte[65];
            unchecked { buffer[0] = id; }

            for (int i = 0; i < 10; i++)
            {
                message = message.Replace("%" + i, "&" + i);
                message = message.Replace("&" + i + " &", " &");
            }
            for (char ch = 'a'; ch <= 'f'; ch++)
            {
                message = message.Replace("%" + ch, "&" + ch);
                message = message.Replace("&" + ch + " &", " &");
            }

            message = message.Replace("$name", "$" + name);
            message = message.Replace("$date", DateTime.Now.ToString("yyyy-MM-dd"));
            message = message.Replace("$time", DateTime.Now.ToString("HH:mm:ss"));
            message = message.Replace("$ip", ip);
            message = message.Replace("$color", color);
            message = message.Replace("$rank", group.Name);
            message = message.Replace("$level", level.name);
            message = message.Replace("$deaths", overallDeath.ToString());
            message = message.Replace("$money", money.ToString());
            message = message.Replace("$blocks", overallBlocks.ToString());
            message = message.Replace("$first", firstLogin.ToString());
            message = message.Replace("$kicked", totalKicked.ToString());
            message = message.Replace("$server", Server.name);
            message = message.Replace("$motd", Server.motd);

            message = message.Replace("$irc", Server.ircServer + " > " + Server.ircChannel);

            if (Server.parseSmiley && parseSmiley)
            {
                message = message.Replace(":)", "(darksmile)");
                message = message.Replace(":D", "(smile)");
                message = message.Replace("<3", "(heart)");
            }

            byte[] stored = new byte[1];

            stored[0] = 1;
            message = message.Replace("(darksmile)", enc.GetString(stored));
            stored[0] = 2;
            message = message.Replace("(smile)", enc.GetString(stored));
            stored[0] = 3;
            message = message.Replace("(heart)", enc.GetString(stored));
            stored[0] = 4;
            message = message.Replace("(diamond)", enc.GetString(stored));
            stored[0] = 7;
            message = message.Replace("(bullet)", enc.GetString(stored));
            stored[0] = 8;
            message = message.Replace("(hole)", enc.GetString(stored));
            stored[0] = 11;
            message = message.Replace("(male)", enc.GetString(stored));
            stored[0] = 12;
            message = message.Replace("(female)", enc.GetString(stored));
            stored[0] = 15;
            message = message.Replace("(sun)", enc.GetString(stored));
            stored[0] = 16;
            message = message.Replace("(right)", enc.GetString(stored));
            stored[0] = 17;
            message = message.Replace("(left)", enc.GetString(stored));
            stored[0] = 19;
            message = message.Replace("(double)", enc.GetString(stored));
            stored[0] = 22;
            message = message.Replace("(half)", enc.GetString(stored));
            stored[0] = 24;
            message = message.Replace("(uparrow)", enc.GetString(stored));
            stored[0] = 25;
            message = message.Replace("(downarrow)", enc.GetString(stored));
            stored[0] = 26;
            message = message.Replace("(rightarrow)", enc.GetString(stored));
            stored[0] = 30;
            message = message.Replace("(up)", enc.GetString(stored));
            stored[0] = 31;
            message = message.Replace("(down)", enc.GetString(stored));

            int totalTries = 0;
        retryTag: try
            {
                foreach (string line in Wordwrap(message))
                {
                    string newLine = line;
                    if (newLine.TrimEnd(' ')[newLine.TrimEnd(' ').Length - 1] < '!')
                    {
                        newLine += '\'';
                    }

                    StringFormat(newLine, 64).CopyTo(buffer, 1);
                    SendRaw(13, buffer);
                }
            }
            catch (Exception e)
            {
                message = "&f" + message;
                totalTries++;
                if (totalTries < 10) goto retryTag;
                else Server.ErrorLog(e);
            }
        }
        public void SendMotd()
        {
            byte[] buffer = new byte[130];
            buffer[0] = Server.version;
            StringFormat(Server.name, 64).CopyTo(buffer, 1);
            StringFormat(Server.motd, 64).CopyTo(buffer, 65);
            if (Server.operators.Contains(name.ToLower()) || Server.superOps.Contains(name.ToLower()))
                buffer[129] = 100;
            else
                buffer[129] = 0;
            SendRaw(0, buffer);
        }

        public void SendUserMOTD()
        {
            byte[] buffer = new byte[130];
            Random rand = new Random();
            buffer[0] = Server.version;
            if (level.motd == "ignore") StringFormat(Server.userMOTD[rand.Next(0, Server.userMOTD.Length)], 128).CopyTo(buffer, 1);
            else StringFormat(level.motd, 128).CopyTo(buffer, 1);

            if (Server.operators.Contains(name.ToLower()) || Server.superOps.Contains(name.ToLower())) buffer[129] = 100;
            else buffer[129] = 0;
            SendRaw(0, buffer);
        }

        public void SendMap()
        {
            SendRaw(2);
            byte[] buffer = new byte[level.blocks.Length + 4];
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.blocks.Length)).CopyTo(buffer, 0);
            //ushort xx; ushort yy; ushort zz;

            for (int i = 0; i < level.blocks.Length; ++i)
            {
                buffer[4 + i] = Block.Convert(level.blocks[i]);
            }

            buffer = GZip(buffer);
            int number = (int)Math.Ceiling(((double)buffer.Length) / 1024);
            for (int i = 1; buffer.Length > 0; ++i)
            {
                short length = (short)Math.Min(buffer.Length, 1024);
                byte[] send = new byte[1027];
                HTNO(length).CopyTo(send, 0);
                Buffer.BlockCopy(buffer, 0, send, 2, length);
                byte[] tempbuffer = new byte[buffer.Length - length];
                Buffer.BlockCopy(buffer, length, tempbuffer, 0, buffer.Length - length);
                buffer = tempbuffer;
                send[1026] = (byte)(i * 100 / number);
                SendRaw(3, send);
                if (ip == "127.0.0.1") { }
                else if (Server.updateTimer.Interval > 1000) Thread.Sleep(100);
                else Thread.Sleep(10);
            }
            buffer = new byte[6];
            HTNO((short)level.width).CopyTo(buffer, 0);
            HTNO((short)level.depth).CopyTo(buffer, 2);
            HTNO((short)level.height).CopyTo(buffer, 4);
            SendRaw(4, buffer);
            Loading = false;
        }
        public void SendSpawn(byte id, string name, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos = new ushort[3] { x, y, z }; // This could be remove and not effect the server :/
            rot = new byte[2] { rotx, roty };
            byte[] buffer = new byte[73]; buffer[0] = id;
            StringFormat(name, 64).CopyTo(buffer, 1);
            HTNO(x).CopyTo(buffer, 65);
            HTNO(y).CopyTo(buffer, 67);
            HTNO(z).CopyTo(buffer, 69);
            buffer[71] = rotx; buffer[72] = roty;
            SendRaw(7, buffer);
        }
        public void SendPos(byte id, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos[0] = x; pos[1] = y; pos[2] = z;
            rot[0] = rotx; rot[1] = roty;

            /*
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };*/
            byte[] buffer = new byte[9]; buffer[0] = id;
            HTNO(x).CopyTo(buffer, 1);
            HTNO(y).CopyTo(buffer, 3);
            HTNO(z).CopyTo(buffer, 5);
            buffer[7] = rotx; buffer[8] = roty;
            SendRaw(8, buffer);
        }
        //TODO: Figure a way to SendPos without changing rotation
        public void SendDie(byte id) { SendRaw(0x0C, new byte[1] { id }); }
        public void SendBlockchange(ushort x, ushort y, ushort z, byte type)
        {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.width || y >= level.depth || z >= level.height) return;

            byte[] buffer = new byte[7];
            HTNO(x).CopyTo(buffer, 0);
            HTNO(y).CopyTo(buffer, 2);
            HTNO(z).CopyTo(buffer, 4);
            buffer[6] = Block.Convert(type);
            SendRaw(6, buffer);
        }
        void SendKick(string message) { SendRaw(14, StringFormat(message, 64)); }
        void SendPing() { /*pingDelay = 0; pingDelayTimer.Start();*/ SendRaw(1); }
        void UpdatePosition()
        {

            //pingDelayTimer.Stop();

            // Shameless copy from JTE's Server
            byte changed = 0;   //Denotes what has changed (x,y,z, rotation-x, rotation-y)
            // 0 = no change - never happens with this code.
            // 1 = position has changed
            // 2 = rotation has changed
            // 3 = position and rotation have changed
            // 4 = Teleport Required (maybe something to do with spawning)
            // 5 = Teleport Required + position has changed
            // 6 = Teleport Required + rotation has changed
            // 7 = Teleport Required + position and rotation has changed
            //NOTE: Players should NOT be teleporting this often. This is probably causing some problems.
            if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2])
                changed |= 1;

            if (oldrot[0] != rot[0] || oldrot[1] != rot[1])
            {
                changed |= 2;
            }
            if (Math.Abs(pos[0] - basepos[0]) > 32 || Math.Abs(pos[1] - basepos[1]) > 32 || Math.Abs(pos[2] - basepos[2]) > 32)
                changed |= 4;

            if (oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2] && (basepos[0] != pos[0] || basepos[1] != pos[1] || basepos[2] != pos[2]))
                changed |= 4;


            byte[] buffer = new byte[0]; byte msg = 0;
            if ((changed & 4) != 0)
            {
                msg = 8; //Player teleport - used for spawning or moving too fast
                buffer = new byte[9]; buffer[0] = id;
                HTNO(pos[0]).CopyTo(buffer, 1);
                HTNO(pos[1]).CopyTo(buffer, 3);
                HTNO(pos[2]).CopyTo(buffer, 5);
                buffer[7] = rot[0]; buffer[8] = rot[1];
            }
            else if (changed == 1)
            {
                try
                {
                    msg = 10; //Position update
                    buffer = new byte[4]; buffer[0] = id;
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[0] - oldpos[0])), 0, buffer, 1, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[1] - oldpos[1])), 0, buffer, 2, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[2] - oldpos[2])), 0, buffer, 3, 1);
                }
                catch { }
            }
            else if (changed == 2)
            {
                msg = 11; //Orientation update
                buffer = new byte[3]; buffer[0] = id;
                buffer[1] = rot[0]; buffer[2] = rot[1];
            }
            else if (changed == 3)
            {
                try
                {
                    msg = 9; //Position and orientation update
                    buffer = new byte[6]; buffer[0] = id;
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[0] - oldpos[0])), 0, buffer, 1, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[1] - oldpos[1])), 0, buffer, 2, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes((sbyte)(pos[2] - oldpos[2])), 0, buffer, 3, 1);
                    buffer[4] = rot[0]; buffer[5] = rot[1];
                }
                catch { }
            }

            oldpos = pos; oldrot = rot;
            if (changed != 0)
                foreach (Player p in players)
                {
                    if (p != this && p.level == level)
                    {
                        p.SendRaw(msg, buffer);
                    }
                }
        }
        #endregion
        #region == GLOBAL MESSAGES ==
        public static void GlobalBlockchange(Level level, ushort x, ushort y, ushort z, byte type)
        {
            players.ForEach(delegate (Player p) { if (p.level == level) { p.SendBlockchange(x, y, z, type); } });
        }
        public static void GlobalChat(Player from, string message) { GlobalChat(from, message, true); }
        public static void GlobalChat(Player from, string message, bool showname)
        {
            if (showname) { message = from.color + from.prefix + from.name + ": &f" + message; }
            players.ForEach(delegate (Player p) { if (p.level.worldChat) p.SendChat(p, message); });
        }
        public static void GlobalChatLevel(Player from, string message, bool showname)
        {
            if (showname) { message = "<Level>" + from.color + from.prefix + from.name + ": &f" + message; }
            players.ForEach(delegate (Player p) { if (p.level == from.level) p.SendChat(p, Server.DefaultColor + message); });
        }
        public static void GlobalChatWorld(Player from, string message, bool showname)
        {
            if (showname) { message = "<World>" + from.color + from.prefix + from.name + ": &f" + message; }
            players.ForEach(delegate (Player p) { if (p.level.worldChat) p.SendChat(p, Server.DefaultColor + message); });
        }
        public static void GlobalMessage(string message)
        {
            message = message.Replace("%", "&");
            players.ForEach(delegate (Player p) { if (p.level.worldChat) p.SendMessage(message); });
        }
        public static void GlobalMessageLevel(Level l, string message)
        {
            players.ForEach(delegate (Player p) { if (p.level == l) p.SendMessage(message); });
        }
        public static void GlobalMessageOps(string message)
        {
            players.ForEach(delegate (Player p)
            {
                if (p.group == Group.Find("operator") || p.group == Group.Find("superOp"))
                {
                    p.SendMessage(message);
                }
            });
        }
        public static void GlobalSpawn(Player from, ushort x, ushort y, ushort z, byte rotx, byte roty, bool self)
        {
            players.ForEach(delegate (Player p)
            {
                if (p.Loading && p != from) { return; }
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendSpawn(from.id, from.color + from.name, x, y, z, rotx, roty); }
                else if (self)
                {
                    if (!p.ignorePermission)
                    {
                        p.pos = new ushort[3] { x, y, z }; p.rot = new byte[2] { rotx, roty };
                        p.oldpos = p.pos; p.basepos = p.pos; p.oldrot = p.rot;
                        unchecked { p.SendSpawn((byte)-1, from.color + from.name, x, y, z, rotx, roty); }
                    }
                }
            });
        }
        public static void GlobalDie(Player from, bool self)
        {
            players.ForEach(delegate (Player p)
            {
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendDie(from.id); }
                else if (self) { unchecked { p.SendDie((byte)-1); } }
            });
        }
        public static void GlobalUpdate() { players.ForEach(delegate (Player p) { if (!p.hidden) { p.UpdatePosition(); } }); }
        #endregion
        #region == DISCONNECTING ==
        public void Disconnect() { LeftGame(); }
        public void Kick(string kickString) { LeftGame(kickString); }

        public void LeftGame(string kickString = "")
        {
            if (disconnected)
            {
                if (connections.Contains(this)) connections.Remove(this);
                return;
            }
            disconnected = true;
            pingTimer.Stop();
            afkTimer.Stop();
            afkCount = 0;
            afkStart = DateTime.Now;
            if (Server.afkset.Contains(name)) Server.afkset.Remove(name);

            if (kickString == "") kickString = "Disconnected.";

            SendKick(kickString);

            if (loggedIn)
            {
                GlobalDie(this, false);
                if (kickString == "Disconnected." || kickString.IndexOf("Server shutdown") != -1)
                {
                    if (!hidden) { GlobalChat(this, "&c- " + color + prefix + name + Server.DefaultColor + " disconnected.", false); }
                    IRCBot.Say(name + " left the game.");
                    Server.s.Log(name + " disconnected.");
                }
                else
                {
                    totalKicked++;
                    GlobalChat(this, "&c- " + color + prefix + name + Server.DefaultColor + " kicked (" + kickString + ").", false);
                    IRCBot.Say(name + " kicked (" + kickString + ").");
                    Server.s.Log(name + " kicked (" + kickString + ").");
                }

                try { Save(); } catch (Exception e) { Server.ErrorLog(e); }

                playerCon.Close();
                playerCon = null;

                try
                {
                    OfflineUndoName[LastLogoff] = name;

                    OfflineUndo[LastLogoff] = new List<UndoPos>();
                    foreach (UndoPos uP in UndoBuffer)
                    {
                        OfflineUndo[LastLogoff].Add(uP);
                    }
                    LastLogoff++;
                    if (LastLogoff == 300) LastLogoff = 0;
                }
                catch (Exception e) { Server.ErrorLog(e); }

                players.Remove(this);
                Server.s.PlayerListUpdate();
                left.Add(name.ToLower(), ip);
            }
            else
            {
                connections.Remove(this);
                Server.s.Log(ip + " disconnected.");
            }
        }

        #endregion
        #region == CHECKING ==
        public static List<Player> GetPlayers() { return new List<Player>(players); }
        public static bool Exists(string name)
        {
            foreach (Player p in players)
            { if (p.name.ToLower() == name.ToLower()) { return true; } }
            return false;
        }
        public static bool Exists(byte id)
        {
            foreach (Player p in players)
            { if (p.id == id) { return true; } }
            return false;
        }
        public static Player Find(string name)
        {
            Player tempPlayer = null; bool returnNull = false;

            foreach (Player p in players)
            {
                if (p.name.ToLower() == name.ToLower()) return p;
                if (p.name.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempPlayer == null) tempPlayer = p;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempPlayer != null) return tempPlayer;
            return null;
        }
        public static Group GetGroup(string name)
        {
            if (Server.banned.Contains(name)) { return Group.Find("banned"); }
            if (Server.builders.Contains(name)) { return Group.Find("builder"); }
            if (Server.advbuilders.Contains(name)) { return Group.Find("advbuilder"); }
            if (Server.operators.Contains(name)) { return Group.Find("operator"); }
            if (Server.superOps.Contains(name)) { return Group.Find("superOp"); }

            return Group.standard;
        }
        public static string GetColor(string name) { return GetGroup(name).Color; }
        #endregion
        #region == OTHER ==
        static byte FreeId()
        {
            for (byte i = 0; i < Server.players; ++i)
            {
                foreach (Player p in players)
                {
                    if (p.id == i) { goto Next; }
                }
                return i;
            Next: continue;
            }
            unchecked { return (byte)-1; }
        }
        static byte[] StringFormat(string str, int size)
        {
            byte[] bytes = enc.GetBytes(str.PadRight(size).Substring(0, size));
            return bytes;
        }
        static List<string> Wordwrap(string message)
        {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");

            int limit = 64; string color = "";

            while (message.Length > 0)
            {
                //if (Regex.IsMatch(message, "&a")) break;

                if (lines.Count > 0)
                {
                    if (message[0].ToString() == "&")
                        message = "> " + message.Trim();
                    else
                        message = "> " + color + message.Trim();
                }

                if (message.IndexOf("&") == message.IndexOf("&", message.IndexOf("&") + 1) - 2)
                    message = message.Remove(message.IndexOf("&"), 2);

                if (message.Length <= limit) { lines.Add(message); break; }
                for (int i = limit - 1; i > limit - 9; --i)
                    if (message[i] == ' ')
                    {
                        lines.Add(message.Substring(0, i));
                        goto Next;
                    }

                retry:
                if (message.Length == 0 || limit == 0) { return lines; }

                try
                {
                    if (message.Substring(limit - 2, 1) == "&" || message.Substring(limit - 1, 1) == "&")
                    {
                        message = message.Remove(limit - 2, 1);
                        limit -= 2;
                        goto retry;
                    }
                    else if (message[limit - 1] < 32 || message[limit - 1] > 127)
                    {
                        message = message.Remove(limit - 1, 1);
                        limit -= 1;
                        //goto retry;
                    }
                }
                catch { return lines; }
                lines.Add(message.Substring(0, limit));

            Next: message = message.Substring(lines[lines.Count - 1].Length);
                if (lines.Count == 1) limit = 60;

                int index = lines[lines.Count - 1].LastIndexOf('&');
                if (index != -1)
                {
                    if (index < lines[lines.Count - 1].Length - 1)
                    {
                        char next = lines[lines.Count - 1][index + 1];
                        if ("0123456789abcdef".IndexOf(next) != -1) { color = "&" + next; }
                        if (index == lines[lines.Count - 1].Length - 1)
                        {
                            lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 2);
                        }
                    }
                    else if (message.Length != 0)
                    {
                        char next = message[0];
                        if ("0123456789abcdef".IndexOf(next) != -1)
                        {
                            color = "&" + next;
                        }
                        lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 1);
                        message = message.Substring(1);
                    }
                }
            }
            return lines;
        }
        public static bool ValidName(string name)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890._";
            foreach (char ch in name) { if (allowedchars.IndexOf(ch) == -1) { return false; } }
            return true;
        }
        public static byte[] GZip(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true);
            gs.Write(bytes, 0, bytes.Length);
            gs.Close();
            gs.Dispose();
            ms.Position = 0;
            bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();
            ms.Dispose();
            return bytes;
        }
        #endregion
        #region == Host <> Network ==
        public static byte[] HTNO(ushort x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        public static ushort NTHO(byte[] x, int offset)
        {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }
        public static byte[] HTNO(short x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        #endregion

        bool CheckBlockSpam()
        {
            if (spamBlockLog.Count >= spamBlockCount)
            {
                DateTime oldestTime = spamBlockLog.Dequeue();
                double spamTimer = DateTime.Now.Subtract(oldestTime).TotalSeconds;
                if (spamTimer < spamBlockTimer)
                {
                    Kick("You were kicked by antigrief system. Slow down.");
                    SendMessage(C.red + name + " was kicked for suspected griefing.");
                    Server.s.Log(name + " was kicked for block spam (" + spamBlockCount + " blocks in " + spamTimer + " seconds)");
                    return true;
                }
            }
            spamBlockLog.Enqueue(DateTime.Now);
            return false;
        }
    }
}