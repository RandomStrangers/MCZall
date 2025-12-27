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
using System;
using System.IO;

namespace MCZall
{
    public static class Properties
    {
        public static void Load(string givenPath, bool skipsalt = false)
        {
            if (!skipsalt)
            {
                Server.salt = "";
                string rndchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random rnd = new Random();
                for (int i = 0; i < 16; ++i) { Server.salt += rndchars[rnd.Next(rndchars.Length)]; }
            }

            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        string color;

                        int foundLimit;

                        switch (key.ToLower())
                        {
                            case "server-name":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                {
                                    Server.name = value;
                                }
                                else { Server.s.Log("server-name invalid! setting to default."); }
                                break;
                            case "motd":
                                if (ValidString(value, "![]&:.,{}~-+()?_/\\ "))
                                {
                                    Server.motd = value;
                                }
                                else { Server.s.Log("motd invalid! setting to default."); }
                                break;
                            case "port":
                                try { Server.port = Convert.ToInt32(value); }
                                catch { Server.s.Log("port invalid! setting to default."); }
                                break;
                            case "verify-names":
                                Server.verify = value.ToLower() == "true";
                                break;
                            case "public":
                                Server.pub = value.ToLower() == "true";
                                break;
                            case "world-chat":
                                Server.worldChat = value.ToLower() == "true";
                                break;
                            case "guest-goto":
                                Server.guestGoto = value.ToLower() == "true";
                                break;
                            case "max-players":
                                try
                                {
                                    if (Convert.ToByte(value) > 64)
                                    {
                                        value = "64"; Server.s.Log("Max players has been lowered to 64.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.s.Log("Max players has been increased to 1.");
                                    }
                                    Server.players = Convert.ToByte(value);
                                }
                                catch { Server.s.Log("max-players invalid! setting to default."); }
                                break;
                            case "max-maps":
                                try
                                {
                                    if (Convert.ToByte(value) > 35)
                                    {
                                        value = "35";
                                        Server.s.Log("Max maps has been lowered to 35.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                        Server.s.Log("Max maps has been increased to 1.");
                                    }
                                    Server.maps = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Server.s.Log("max-maps invalid! setting to default.");
                                }
                                break;
                            case "irc":
                                Server.irc = value.ToLower() == "true";
                                break;
                            case "irc-server":
                                Server.ircServer = value;
                                break;
                            case "irc-nick":
                                Server.ircNick = value;
                                break;
                            case "irc-channel":
                                Server.ircChannel = value;
                                break;
                            case "irc-port":
                                try
                                {
                                    Server.ircPort = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Server.s.Log("irc-port invalid! setting to default.");
                                }
                                break;
                            case "irc-identify":
                                try
                                {
                                    Server.ircIdentify = Convert.ToBoolean(value);
                                }
                                catch
                                {
                                    Server.s.Log("irc-identify boolean value invalid! Setting to the default of: " + Server.ircIdentify + ".");
                                }
                                break;
                            case "irc-password":
                                Server.ircPassword = value;
                                break;
                            case "anti-tunnels":
                                Server.antiTunnel = value.ToLower() == "true";
                                break;
                            case "max-depth":
                                try
                                {
                                    Server.maxDepth = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Server.s.Log("maxDepth invalid! setting to default.");
                                }
                                break;

                            case "overload":
                                try
                                {
                                    if (Convert.ToInt16(value) > 5000)
                                    {
                                        value = "4000";
                                        Server.s.Log("Max overload is 5000.");
                                    }
                                    else if (Convert.ToInt16(value) < 500)
                                    {
                                        value = "500";
                                        Server.s.Log("Min overload is 500");
                                    }
                                    Server.Overload = Convert.ToInt16(value);
                                }
                                catch
                                {
                                    Server.s.Log("Overload invalid! setting to default.");
                                }
                                break;

                            case "rplimit":
                                try { Server.rpLimit = Convert.ToInt16(value); } catch { Server.s.Log("rpLimit invalid! setting to default."); }
                                break;
                            case "rplimit-norm":
                                try { Server.rpNormLimit = Convert.ToInt16(value); } catch { Server.s.Log("rpLimit-norm invalid! setting to default."); }
                                break;

                            case "report-back":
                                Server.reportBack = value.ToLower() == "true";
                                break;
                            case "backup-time":
                                if (Convert.ToInt32(value) > 1) { Server.backupInterval = Convert.ToInt32(value); }
                                break;
                            case "console-only":
                                Server.console = value.ToLower() == "true";
                                break;

                            case "physicsrestart":
                                Server.physicsRestart = value.ToLower() == "true";
                                break;
                            case "deathcount":
                                Server.physicsRestart = value.ToLower() == "true";
                                break;

                            case "host":
                                Server.MySQLHost = value;
                                break;
                            case "username":
                                Server.MySQLUsername = value;
                                break;
                            case "password":
                                Server.MySQLPassword = value;
                                break;
                            case "databasename":
                                Server.MySQLDatabaseName = value;
                                break;
                            case "defaultcolor":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.DefaultColor = color; break;
                            case "irc-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.IRCColour = color; break;
                            case "super-limit":
                                try { foundLimit = int.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                Server.maxSuper = foundLimit;
                                break;
                            case "op-limit":
                                try { foundLimit = int.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                Server.maxOp = foundLimit;
                                break;
                            case "adv-limit":
                                try { foundLimit = int.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                Server.maxAdv = foundLimit;
                                break;
                            case "builder-limit":
                                try { foundLimit = int.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                Server.maxBuild = foundLimit;
                                break;
                            case "super-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colSuper = color; break;
                            case "op-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colOp = color; break;
                            case "adv-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colAdv = color; break;
                            case "builder-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colBuild = color; break;
                            case "guest-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colGuest = color; break;
                            case "banned-color":
                                color = C.Parse(value);
                                if (color == "")
                                {
                                    color = C.Name(value); if (color != "") color = value; else { Server.s.Log("Could not find " + value); return; }
                                }
                                Server.colBanned = color; break;
                            case "old-help":
                                try { Server.oldHelp = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;

                            case "cheapmessage":
                                try { Server.cheapMessage = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "rank-super":
                                try { Server.rankSuper = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            /*case "default-rank":
                                try { Group.standard = Group.Find(value); } catch { }
                                break;*/

                            case "afk-minutes":
                                try
                                {
                                    Server.afkminutes = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Server.s.Log("irc-port invalid! setting to default.");
                                }
                                break;

                            case "afk-kick":
                                try { Server.afkkick = Convert.ToInt32(value); } catch { Server.s.Log("irc-port invalid! setting to default."); }
                                break;
                            case "check-updates":
                                try { Server.checkUpdates = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "autoload":
                                try { Server.AutoLoad = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "parse-emotes":
                                try { Server.parseSmiley = bool.Parse(value); } catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
                                break;
                            case "main-name":
                                if (Player.ValidName(value)) Server.level = value;
                                else Server.s.Log("Invalid main name");
                                break;
                        }
                    }
                }
                Server.s.Log("LOADED: " + givenPath);
                Server.s.SettingsUpdate();
                Save(givenPath);
            }
            else Save(givenPath);
        }
        public static bool ValidString(string str, string allowed)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach (char ch in str)
            {
                if (allowedchars.IndexOf(ch) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        static void Save(string givenPath)
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create(givenPath));
                if (givenPath.IndexOf("server") != -1)
                {
                    w.WriteLine("# Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
                    w.WriteLine("#   server-name\t=\tThe name which displays on minecraft.net");
                    w.WriteLine("#   motd\t=\tThe message which displays when a player connects");
                    w.WriteLine("#   port\t=\tThe port to operate from");
                    w.WriteLine("#   console-only\t=\tRun without a GUI (useful for Linux servers with mono)");
                    w.WriteLine("#   verify-names\t=\tVerify the validity of names");
                    w.WriteLine("#   public\t=\tSet to true to appear in the public server list");
                    w.WriteLine("#   max-players\t=\tThe maximum number of connections");
                    w.WriteLine("#   max-maps\t=\tThe maximum number of maps loaded at once");
                    w.WriteLine("#   world-chat\t=\tSet to true to enable world chat");
                    w.WriteLine("#   guest-goto\t=\tSet to true to give guests goto and levels commands");
                    w.WriteLine("#   irc\t=\tSet to true to enable the IRC bot");
                    w.WriteLine("#   irc-nick\t=\tThe name of the IRC bot");
                    w.WriteLine("#   irc-server\t=\tThe server to connect to");
                    w.WriteLine("#   irc-channel\t=\tThe channel to join");
                    w.WriteLine("#   irc-port\t=\tThe port to use to connect");
                    w.WriteLine("#   irc-identify\t=(true/false)\tDo you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
                    w.WriteLine("#   irc-password\t=\tThe password you want to use if you're identifying with nickserv");
                    w.WriteLine("#   anti-tunnels\t=\tStops people digging below max-depth");
                    w.WriteLine("#   max-depth\t=\tThe maximum allowed depth to dig down");
                    w.WriteLine("#   backup-time\t=\tThe number of seconds between automatic backups");
                    w.WriteLine("#   overload\t=\tThe higher this is, the longer the physics is allowed to lag. Default 1500");
                    w.WriteLine();
                    w.WriteLine("#   Host\t=\tThe host name for the database (usually 127.0.0.1)");
                    w.WriteLine("#   Username\t=\tThe username you used to create the database (usually root)");
                    w.WriteLine("#   Password\t=\tThe password set while making the database");
                    w.WriteLine("#   DatabaseName\t=\tThe name of the database stored (Default = MCZall)");
                    w.WriteLine();
                    w.WriteLine("#   defaultColor\t=\tThe color code of the default messages (Default = &e)");
                    w.WriteLine();
                    w.WriteLine("#   Super-limit\t=\tThe limit for building commands for SuperOPs");
                    w.WriteLine("#   Op-limit\t=\tThe limit for building commands for Operators");
                    w.WriteLine("#   Adv-limit\t=\tThe limit for building commands for AdvBuilders");
                    w.WriteLine("#   Builder-limit\t=\tThe limit for building commands for Builders");
                    w.WriteLine();
                    w.WriteLine();
                    w.WriteLine("# Server options");
                    w.WriteLine("server-name = " + Server.name);
                    w.WriteLine("motd = " + Server.motd);
                    w.WriteLine("port = " + Server.port.ToString());
                    w.WriteLine("verify-names = " + Server.verify.ToString().ToLower());
                    w.WriteLine("public = " + Server.pub.ToString().ToLower());
                    w.WriteLine("max-players = " + Server.players.ToString());
                    w.WriteLine("max-maps = " + Server.maps.ToString());
                    w.WriteLine("world-chat = " + Server.worldChat.ToString().ToLower());
                    w.WriteLine("check-updates = " + Server.checkUpdates.ToString());
                    w.WriteLine("autoload = " + Server.AutoLoad.ToString());
                    w.WriteLine("main-name = " + Server.level);
                    w.WriteLine();
                    w.WriteLine("# irc bot options");
                    w.WriteLine("irc = " + Server.irc.ToString().ToLower());
                    w.WriteLine("irc-nick = " + Server.ircNick);
                    w.WriteLine("irc-server = " + Server.ircServer);
                    w.WriteLine("irc-channel = " + Server.ircChannel);
                    w.WriteLine("irc-port = " + Server.ircPort.ToString());
                    w.WriteLine("irc-identify = " + Server.ircIdentify.ToString());
                    w.WriteLine("irc-password = " + Server.ircPassword);
                    w.WriteLine();
                    w.WriteLine("# other options");
                    w.WriteLine("anti-tunnels = " + Server.antiTunnel.ToString().ToLower());
                    w.WriteLine("max-depth = " + Server.maxDepth.ToString());
                    w.WriteLine("overload = " + Server.Overload.ToString());
                    w.WriteLine("rplimit = " + Server.rpLimit.ToString());
                    w.WriteLine("rplimit-norm = " + Server.rpNormLimit.ToString());
                    w.WriteLine("physicsrestart = " + Server.physicsRestart);
                    w.WriteLine("old-help = " + Server.oldHelp);
                    w.WriteLine("deathcount = " + Server.deathcount);
                    w.WriteLine("afk-minutes = " + Server.afkminutes);
                    w.WriteLine("afk-kick = " + Server.afkkick);
                    w.WriteLine("parse-emotes = " + Server.parseSmiley);
                    w.WriteLine();
                    w.WriteLine("# backup options");
                    w.WriteLine("backup-time = " + Server.backupInterval);
                    w.WriteLine();
                    w.WriteLine("#Error logging");
                    w.WriteLine("report-back = " + Server.reportBack.ToString().ToLower());
                    w.WriteLine();
                    w.WriteLine("#MySQL information");
                    w.WriteLine("Host = " + Server.MySQLHost);
                    w.WriteLine("Username = " + Server.MySQLUsername);
                    w.WriteLine("Password = " + Server.MySQLPassword);
                    w.WriteLine("DatabaseName = " + Server.MySQLDatabaseName);
                    w.WriteLine();
                    w.WriteLine("#Colors");
                    w.WriteLine("defaultColor = " + Server.DefaultColor);
                    w.WriteLine("irc-color = " + Server.IRCColour);
                }
                else if (givenPath.IndexOf("rank") != -1)
                {
                    w.WriteLine("#Building command limits");
                    w.WriteLine("super-limit = " + Server.maxSuper);
                    w.WriteLine("op-limit = " + Server.maxOp);
                    w.WriteLine("adv-limit = " + Server.maxAdv);
                    w.WriteLine("builder-limit = " + Server.maxBuild);
                    w.WriteLine("#Colours");
                    w.WriteLine("super-color = " + Server.colSuper);
                    w.WriteLine("op-color = " + Server.colOp);
                    w.WriteLine("adv-color = " + Server.colAdv);
                    w.WriteLine("builder-color = " + Server.colBuild);
                    w.WriteLine("guest-color = " + Server.colGuest);
                    w.WriteLine("banned-color = " + Server.colBanned);
                    w.WriteLine();
                    w.WriteLine("#Misc");
                    w.WriteLine("cheapmessage = " + Server.cheapMessage.ToString().ToLower());
                    w.WriteLine("rank-super = " + Server.rankSuper.ToString().ToLower());
                    //try { w.WriteLine("default-rank = " + Group.standard.name); } catch { w.WriteLine("default-rank = guest"); }
                }
                w.Flush();
                w.Close();
                w.Dispose();

                Server.s.Log("SAVED: " + givenPath);
            }
            catch
            {
                Server.s.Log("SAVE FAILED! " + givenPath);
            }
        }
    }
}
