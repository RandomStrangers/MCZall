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
using MonoTorrent.Client;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MCZall
{
    public class Server
    {
        public delegate void LogHandler(string message);
        public delegate void HeartBeatHandler();
        public delegate void MessageEventHandler(string message);
        public delegate void PlayerListHandler(List<Player> playerList);
        public delegate void VoidHandler();
        public delegate void CommandHandler(string message);

        public event LogHandler OnLog;
        public event HeartBeatHandler HeartBeatFail;
        public event MessageEventHandler OnURLChange;
        public event PlayerListHandler OnPlayerListChange;
        public event VoidHandler OnSettingsUpdate;
        public event CommandHandler OnCommand;

        public static MySqlConnection mysqlCon;

        public static string ZallState = "alive";

        public static int speedPhysics = 250;

        public static string Version { get { return InternalVersion; } }
        public const string InternalVersion = "35.0.0.0";

        public static Socket listen;
        public static Process process = Process.GetCurrentProcess();
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
        //static System.Timers.Timer heartbeatTimer = new System.Timers.Timer(60000);     //Every 45 seconds
        static readonly System.Timers.Timer messageTimer = new System.Timers.Timer(60000 * 5);   //Every 5 mins
        public static System.Timers.Timer cloneTimer = new System.Timers.Timer(5000);

        public static Thread physThread;
        public static bool physPause;
        public static DateTime physResume = DateTime.Now;
        public static System.Timers.Timer physTimer = new System.Timers.Timer(1000);
        // static Thread botsThread;

        public static PlayerList operators;
        public static PlayerList superOps;
        public static PlayerList banned;
        public static PlayerList bannedIP;
        public static PlayerList builders;
        public static PlayerList advbuilders;
        public static PlayerList bot;
        public static MapGenerator MapGen;

        public static PerformanceCounter PCCounter;
        public static PerformanceCounter ProcessCounter;

        public static PlayerList ircControllers;

        public static Level mainLevel;
        public static List<Level> levels;
        public static List<string> afkset = new List<string>();
        public static List<string> afkmessages = new List<string>();
        public static List<string> messages = new List<string>();

        public static DateTime timeOnline;

        //Lua lua;
        #region Server Settings
        public const byte version = 7;
        public static string salt = "";

        public static string name = "[MCZall] Default";
        public static string motd = "Welcome!";
        public static byte players = 12;
        public static byte maps = 5;
        public static int port = 25565;
        public static bool pub = true;
        public static bool verify = true;
        public static bool worldChat = true;
        public static bool guestGoto = false;

        public static string[] userMOTD;

        public static string level = "main";
        public static string errlog = "error.log";

        public static bool console = false;
        public static bool reportBack = true;

        public static bool irc = false;
        public static int ircPort = 6667;
        public static string ircNick = "MCZall_Minecraft_Bot";
        public static string ircServer = "irc.esper.net";
        public static string ircChannel = "#changethis";
        public static bool ircIdentify = false;
        public static string ircPassword = "";

        public static bool antiTunnel = true;
        public static byte maxDepth = 4;
        public static int Overload = 1500;
        public static int rpLimit = 500;
        public static int rpNormLimit = 10000;
        public static int backupInterval = 300;
        public static bool physicsRestart = true;
        public static bool deathcount = true;
        public static bool AutoLoad = false;
        public static int physUndo = 60000;
        public static bool cheapMessage = true;
        public static bool rankSuper = true;
        public static bool oldHelp = false;
        public static bool parseSmiley = true;

        public static bool checkUpdates = true;

        public static string MySQLHost = "127.0.0.1";
        public static string MySQLUsername = "root";
        public static string MySQLPassword = "password";
        public static string MySQLDatabaseName = "MCZallDB";

        public static string DefaultColor = "&e";
        public static string IRCColour = "&5";

        public static int afkminutes = 10;
        public static int afkkick = 45;

        public static int maxSuper = 65536;
        public static int maxOp = 2500;
        public static int maxAdv = 1200;
        public static int maxBuild = 400;

        public static string colSuper = "&e";
        public static string colOp = "&c";
        public static string colAdv = "&3";
        public static string colBuild = "&2";
        public static string colGuest = "&7";
        public static string colBanned = "&7";

        public static bool shuttingDown = false;
        #endregion

        public static MainLoop ml;
        public static Server s;
        public Server()
        {
            //lua = new Lua();
            //lua["server"] = this;
            ml = new MainLoop();
            s = this;

        }
        public void Start()
        {
            Log("Starting Server");
            try
            {
                if (!Directory.Exists("properties"))
                    Directory.CreateDirectory("properties");
                if (File.Exists("rank.properties"))
                    File.Move("rank.properties", "properties/command.properties");
                if (File.Exists("server.properties"))
                    File.Move("server.properties", "properties/server.properties");
            }
            catch { }
            Properties.Load("properties/server.properties");
            Properties.Load("properties/rank.properties", true);

            Block.SetBlocks();

            try
            {
                if (!Directory.Exists("bots")) Directory.CreateDirectory("bots");
                if (!Directory.Exists("text files")) Directory.CreateDirectory("text");
                if (File.Exists("rules.txt")) File.Move("rules.txt", "text/rules.txt");
                if (File.Exists("welcome.txt")) File.Move("welcome.txt", "text/welcome.txt");
                if (File.Exists("messages.txt")) File.Move("messages.txt", "text/messages.txt");
                if (File.Exists("externalurl.txt")) File.Move("externalurl.txt", "text/externalurl.txt");
                if (File.Exists("autoload.txt")) File.Move("autoload.txt", "text/autoload.txt");
                if (File.Exists("IRC_Controllers.txt")) File.Move("IRC_Controllers.txt", "ranks/IRC_Controllers.txt");
            }
            catch { }

        retry: if (File.Exists("text/motd.txt"))
            {
                userMOTD = File.ReadAllLines("text/motd.txt");
                string storeName = "", storeMOTD = "";
                for (int i = 0; i < userMOTD.Length; i++)
                {
                    try
                    {
                        storeName = userMOTD[i].Substring(userMOTD[i].IndexOf("NAME: ") + 5, userMOTD[i].IndexOf("MOTD: ") - 5);
                        storeMOTD = userMOTD[i].Substring(userMOTD[i].IndexOf("MOTD: ") + 5);

                        if (storeName.Length < 64) storeName = storeName.PadRight(64, ' ');
                        else if (storeName.Length > 64) storeName = storeName.Substring(0, 63);

                        if (storeMOTD.Length < 64) storeMOTD = storeMOTD.PadRight(64, ' ');
                        else if (storeMOTD.Length > 64) storeMOTD = storeMOTD.Substring(0, 63);

                        userMOTD[i] = storeName + storeMOTD;
                    }
                    catch { }
                }
            }
            else
            {
                File.WriteAllText("text/motd.txt", "NAME: " + name + "MOTD: " + motd);
                goto retry;
            }

            if (File.Exists("text/emotelist.txt"))
            {
                foreach (string s in File.ReadAllLines("text/emotelist.txt"))
                {
                    Player.emoteList.Add(s);
                }
            }
            else
            {
                File.Create("text/emotelist.txt");
            }

            timeOnline = DateTime.Now;

            mysqlCon = new MySqlConnection("Data Source=" + MySQLHost + ";User ID=" + MySQLUsername + ";Password=" + MySQLPassword + ";");
            try { mysqlCon.Open(); }
            catch (Exception e)
            {
                s.Log("MySQL settings have not been set! Please reference the MySQL_Setup.txt file on setting up MySQL!");
                ErrorLog(e);
                //process.Kill();
                return;
            }

            MySqlCommand cmdDatabaseCreate = new MySqlCommand("CREATE DATABASE if not exists " + MySQLDatabaseName, mysqlCon);
        retryTag1: try { cmdDatabaseCreate.ExecuteNonQuery(); } catch (Exception e) { ErrorLog(e); goto retryTag1; }
            mysqlCon.Close();

            mysqlCon = new MySqlConnection("Data Source=" + MySQLHost + ";Database=" + MySQLDatabaseName + ";User ID=" + MySQLUsername + ";Password=" + MySQLPassword + ";Pooling=false");
            mysqlCon.Open();

            cmdDatabaseCreate = new MySqlCommand("CREATE TABLE if not exists Players (ID MEDIUMINT not null auto_increment, Name VARCHAR(20), IP CHAR(15), FirstLogin DATETIME, LastLogin DATETIME, totalLogin MEDIUMINT, Title CHAR(20), TotalDeaths SMALLINT, Money MEDIUMINT UNSIGNED, totalBlocks BIGINT, totalKicked MEDIUMINT, PRIMARY KEY (ID));", mysqlCon);
        retryTag: try { cmdDatabaseCreate.ExecuteNonQuery(); } catch (Exception e) { ErrorLog(e); goto retryTag; }
            cmdDatabaseCreate.Dispose();

            ml.Queue(delegate
            {
                levels = new List<Level>(maps);
                MapGen = new MapGenerator();

                Random random = new Random();

                if (File.Exists("levels/" + level + ".lvl"))
                {
                    mainLevel = new Level("temp", 16, 16, 16, "flat");
                    mainLevel = mainLevel.Load(level);
                    mainLevel.unload = false;
                    mainLevel.physThread.Start();
                    if (mainLevel == null)
                    {
                        if (File.Exists("levels/" + level + ".lvl.backup"))
                        {
                            Log("Attempting to load backup.");
                            File.Copy("levels/" + level + ".lvl.backup", "levels/" + level + ".lvl", true);
                            mainLevel = mainLevel.Load(level);
                            if (mainLevel == null)
                            {
                                Log("BACKUP FAILED!");
                                Console.ReadKey(); return;
                            }
                        }
                        else
                        {
                            Log("BACKUP NOT FOUND!");
                            Console.ReadKey(); return;
                        }

                    }
                }
                else
                {
                    Log("mainlevel not found");
                    mainLevel = new Level(level, 128, 64, 128, "flat")
                    {
                        permissionvisit = LevelPermission.Guest,
                        permissionbuild = LevelPermission.Guest
                    };
                    mainLevel.Save();
                }
                levels.Add(mainLevel);
            });

            ml.Queue(delegate
            {


                // TODO: Administrator group.
                if (File.Exists("ranks/admins.txt"))
                {
                    File.Copy("ranks/admins.txt", "ranks/operators.txt", true);
                    File.Delete("ranks/admins.txt");
                }

                banned = PlayerList.Load("banned.txt");
                bannedIP = PlayerList.Load("banned-ip.txt");
                builders = PlayerList.Load("builders.txt");
                advbuilders = PlayerList.Load("advbuilders.txt");
                operators = PlayerList.Load("operators.txt");
                superOps = PlayerList.Load("uberOps.txt");
                bot = PlayerList.Load("bots.txt");
                ircControllers = PlayerList.Load("IRC_Controllers.txt");


                if (!bot.Contains("flist"))
                {
                    bot.Add("flist");
                    bot.Save("bots.txt", false);
                }
                Command.InitAll();
                GrpCommands.FillRanks();
                Group.InitAll();
            });


            ml.Queue(delegate
            {
                if (File.Exists("text/autoload.txt"))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines("text/autoload.txt");
                        foreach (string line in lines)
                        {
                            //int temp = 0;
                            string _line = line.Trim();
                            try
                            {
                                if (_line == "") { continue; }
                                if (_line[0] == '#') { continue; }
                                int index = _line.IndexOf("=");

                                string key = line.Split('=')[0].Trim();
                                string value;
                                try
                                {
                                    value = line.Split('=')[1].Trim();
                                }
                                catch
                                {
                                    value = "0";
                                }

                                if (!key.Equals("main"))
                                {
                                    Command.all.Find("load").Use(null, key + " " + value);
                                    Level l = Level.Find(key);
                                }
                                else
                                {
                                    try
                                    {
                                        int temp = int.Parse(value);
                                        if (temp >= 0 && temp <= 3)
                                        {
                                            mainLevel.SetPhysics(temp);
                                        }
                                    }
                                    catch
                                    {
                                        s.Log("Physics variable invalid");
                                    }
                                }


                            }
                            catch
                            {
                                s.Log(_line + " failed.");
                            }
                        }
                    }
                    catch
                    {
                        s.Log("autoload.txt error");
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                else
                {
                    Log("autoload.txt does not exist");
                }
            });

            ml.Queue(delegate
            {
                s.Log("Creating listening socket on port " + port + "... ");
                if (Setup())
                {
                    s.Log("Done.");
                }
                else
                {
                    s.Log("Could not create socket connection.  Shutting down.");
                    return;
                }
            });

            ml.Queue(delegate
            {
                updateTimer.Elapsed += delegate
                {
                    Player.GlobalUpdate();
                    PlayerBot.GlobalUpdatePosition();
                };

                updateTimer.Start();


            });
            // Heartbeat code here:

            Heartbeat.Init();

            // END Heartbeat code

            PCCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ProcessCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            PCCounter.BeginInit();
            ProcessCounter.BeginInit();
            PCCounter.NextValue();
            ProcessCounter.NextValue();
            //physThread = new Thread(new ThreadStart(Physics));
            //physThread.Start();

            ml.Queue(delegate
            {
                messageTimer.Elapsed += delegate
                {
                    RandomMessage();
                };
                messageTimer.Start();

                cloneTimer.Elapsed += delegate
                {
                    List<Player> foundClones = new List<Player>();
                    foreach (Player p in Player.players)
                    {
                        foreach (Player pl in Player.players)
                        {
                            if (p.name == pl.name && p != pl)
                            {
                                if (!foundClones.Contains(p)) foundClones.Add(p);
                                if (!foundClones.Contains(pl)) foundClones.Add(pl);
                            }
                        }
                    }

                    if (foundClones.Count != 0)
                        foreach (Player p in foundClones)
                        {
                            p.Kick("Clone!");
                            Player.players.Remove(p);
                        }
                };

                process = Process.GetCurrentProcess();

                if (File.Exists("text/messages.txt"))
                {
                    StreamReader r = File.OpenText("text/messages.txt");
                    while (!r.EndOfStream)
                        messages.Add(r.ReadLine());
                    r.Dispose();
                }
                else File.Create("text/messages.txt").Close();

                if (irc)
                {
                    new IRCBot();
                }

                new AutoSaver(backupInterval);     //2 and a half mins
                //Thread physThread = new Thread(new ThreadStart(Physics));
                //physThread.Start();
                /*if (Server.console)
                {
                    inputThread = new Thread(new ThreadStart(ParseInput));
                    inputThread.Start();
                }*/
            });
        }

        public static bool Setup()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
                listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listen.Bind(endpoint);
                listen.Listen((int)SocketOptionName.MaxConnections);

                listen.BeginAccept(new AsyncCallback(Accept), null);
                return true;
            }
            catch (SocketException e) { ErrorLog(e); return false; }
            catch (Exception e) { ErrorLog(e); return false; }
        }

        static void Accept(IAsyncResult result)
        {
            if (shuttingDown == false)
            {
                // found information: http://www.codeguru.com/csharp/csharp/cs_network/sockets/article.php/c7695
                // -Descention
                try
                {
                    new Player(listen.EndAccept(result));
                    s.Log("New Connection");
                    listen.BeginAccept(new AsyncCallback(Accept), null);
                }
                catch (SocketException e)
                {
                    //s.Close();
                    ErrorLog(e);
                }
                catch (Exception e)
                {
                    //s.Close(); 
                    ErrorLog(e);
                }
            }
        }

        public static void Exit()
        {
            List<string> players = new List<string>();
            foreach (Player p in Player.players) { p.Save(); players.Add(p.name); }
            foreach (string p in players) { Player.Find(p).Kick("Server shutdown. Rejoin in 10 seconds."); }

            //Player.players.ForEach(delegate(Player p) { p.Kick("Server shutdown. Rejoin in 10 seconds."); });
            Player.connections.ForEach(delegate (Player p) { p.Kick("Server shutdown. Rejoin in 10 seconds."); });
            shuttingDown = true;
            listen.Close();

            mysqlCon.Close();
        }

        public void PlayerListUpdate()
        {
            s.OnPlayerListChange?.Invoke(Player.players);
        }

        public void FailBeat()
        {
            HeartBeatFail?.Invoke();
        }

        public void UpdateUrl(string url)
        {
            OnURLChange?.Invoke(url);
        }

        public void Log(string message)
        {
            OnLog?.Invoke(DateTime.Now.ToString("(HH:mm:ss) ") + message);
            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }

        public void CommandUsed(string message)
        {
            OnCommand?.Invoke(DateTime.Now.ToString("(HH:mm:ss) ") + message);
            Logger.Write(DateTime.Now.ToString("(HH:mm:ss) ") + message + Environment.NewLine);
        }

        public static void ErrorLog(string message)
        {
            if (errlog == "") { Console.WriteLine(DateTime.Now.ToString("(HH:mm:ss) ") + "ERROR!"); }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("(HH:mm:ss) ") + "ERROR! See \"" + errlog + "\" for more information.");
                StreamWriter sw = File.AppendText(errlog);
                sw.WriteLine(DateTime.Now.ToString("(HH:mm:ss)"));
                sw.WriteLine(message); sw.Close();
            }
        }

        public static void ErrorLog(Exception ex)
        {
            Logger.WriteError(ex);
        }

        public static void ParseInput(bool console = true, string sentCMD = null, string sentMsg = null)        //Handle console commands
        {
            string cmd;
            string msg;
            while (true)
            {
                string input;
                if (console == true) { input = Console.ReadLine(); } else { input = sentCMD + sentMsg; }
                if (input == null)
                    continue;
                cmd = input.Split(' ')[0];
                if (input.Split(' ').Length > 1)
                {
                    //msg = input.Substring(input.IndexOf(' ')).Trim();
                }
                else
                {
                    msg = "";

                    try
                    {
                        switch (cmd)
                        {
                            case "kick":
                                Command.all.Find("kick").Use(null, msg); break;
                            case "ban":
                                Command.all.Find("ban").Use(null, msg); break;
                            case "banip":
                                Command.all.Find("banip").Use(null, msg); break;
                            case "resetbot":
                                Command.all.Find("resetbot").Use(null, msg); break;
                            case "save":
                                Command.all.Find("save").Use(null, msg); break;
                            case "say":
                                if (!msg.Equals(""))
                                {
                                    if (Properties.ValidString(msg, "![]&:.,{}~-+()?_/\\@%$ "))
                                    {
                                        Player.GlobalMessage(msg);
                                    }
                                    else
                                    {
                                        Console.WriteLine("bad char in say");
                                    }
                                }
                                break;
                            case "guest":
                                Command.all.Find("guest").Use(null, msg); break;
                            case "builder":
                                Command.all.Find("builder").Use(null, msg); break;
                            case "help":
                                Console.WriteLine("ban, banip, builder, guest, kick, resetbot, save, say");
                                break;
                            default:
                                Console.WriteLine("No such command!"); break;
                        }
                    }
                    catch (Exception e) { ErrorLog(e); }
                }
            }
        }

        public static void RandomMessage()
        {
            if (Player.Number != 0 && messages.Count > 0)
                Player.GlobalMessage(messages[new Random().Next(0, messages.Count)]);
        }

        internal void SettingsUpdate()
        {
            OnSettingsUpdate?.Invoke();
        }

        public static string FindColor(string Username)
        {
            if (banned.Contains(Username)) return Group.Find("banned").Color;
            else if (builders.Contains(Username)) return Group.Find("builder").Color;
            else if (advbuilders.Contains(Username)) return Group.Find("advbuilder").Color;
            else if (operators.Contains(Username)) return Group.Find("operator").Color;
            else if (superOps.Contains(Username)) return Group.Find("superop").Color;
            return Group.Find("guest").Color;
        }
    }
}