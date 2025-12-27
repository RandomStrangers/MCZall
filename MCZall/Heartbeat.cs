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
using System.Net;
using System.Text;

namespace MCZall
{

    public class Heartbeat
    {
        //static int _timeout = 60 * 1000;

        static string hash;
        public static string serverURL;
        static string staticVars;

        //static BackgroundWorker worker;
        static HttpWebRequest request;

        static readonly System.Timers.Timer heartbeatTimer = new System.Timers.Timer(500);

        public static void Init()
        {
            staticVars = "port=" + Server.port +
                         "&max=" + Server.players +
                         "&name=" + UrlEncode(Server.name) +
                         "&public=" + Server.pub +
                         "&version=" + Server.version;

            //Pump(Beat.Minecraft);
            //Pump(Beat.TChalo);

            heartbeatTimer.Elapsed += delegate
            {
                heartbeatTimer.Interval = 55000;
                Pump(Beat.Minecraft);
                Pump(Beat.TChalo);
            }; heartbeatTimer.Start();
        }
        public static bool Pump(Beat type)
        {
            string postVars = staticVars;

            string url = "http://www.minecraft.net/heartbeat.jsp";
            int totalTries = 0;
        retry: try
            {
                int hidden = 0;
                totalTries++;
                // append additional information as needed
                switch (type)
                {
                    case Beat.Minecraft:
                        postVars += "&salt=" + Server.salt;
                        goto default;
                    case Beat.TChalo:
                        if (hash == null)
                            throw new Exception("Hash not set");

                        url = "http://minecraft.tchalo.com/announce.php";

                        // build list of current players in server
                        if (Player.Number > 0)
                        {
                            string players = "";
                            foreach (Player p in Player.players)
                            {
                                if (p.hidden)
                                {
                                    hidden++;
                                    continue;
                                }
                                players += p.name + ",";
                            }
                            if (Player.Number - hidden > 0)
                                postVars += "&players=" + players.Substring(0, players.Length - 1);
                        }

                        string worlds = "";
                        foreach (Level l in Server.levels)
                        {
                            worlds += l.name + ",";
                            postVars += "&worlds=" + worlds.Substring(0, worlds.Length - 1);
                        }

                        postVars += "&motd=" + UrlEncode(Server.motd) +
                                "&hash=" + hash +
                                "&data=" + Server.Version + "," + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                                "&server=MCZall" +
                                "&details=Running MCZall version " + Server.Version;

                        goto default;
                    default:
                        postVars += "&users=" + (Player.Number - hidden);
                        break;

                }

                request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                byte[] formData = Encoding.ASCII.GetBytes(postVars);
                request.ContentLength = formData.Length;
                request.Timeout = 15000;

            retryStream: try
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(formData, 0, formData.Length);
                        requestStream.Close();
                    }
                }
                catch (WebException e)
                {
                    //Server.ErrorLog(e);
                    if (e.Status == WebExceptionStatus.Timeout)
                    {
                        goto retryStream;
                        //throw new WebException("Failed during request.GetRequestStream()", e.InnerException, e.Status, e.Response);
                    }
                }

                if (hash == null)
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                        {
                            string line = responseReader.ReadLine();
                            hash = line.Substring(line.LastIndexOf('=') + 1);
                            serverURL = line;

                            Server.s.UpdateUrl(serverURL);
                            File.WriteAllText("text/externalurl.txt", serverURL);
                            Server.s.Log("URL found: " + serverURL);
                        }
                    }
                }
                //Server.s.Log(string.Format("Heartbeat: {0}", type));
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    Pump(type);
                }
            }
            catch
            {
                if (totalTries < 3) goto retry;
                return false;
            }
            finally { request.Abort(); }
            return true;
        }

        public static string UrlEncode(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if ((input[i] >= '0' && input[i] <= '9') ||
                    (input[i] >= 'a' && input[i] <= 'z') ||
                    (input[i] >= 'A' && input[i] <= 'Z') ||
                    input[i] == '-' || input[i] == '_' || input[i] == '.' || input[i] == '~')
                {
                    output.Append(input[i]);
                }
                else if (Array.IndexOf(reservedChars, input[i]) != -1)
                {
                    output.Append('%').Append(((int)input[i]).ToString("X"));
                }
            }
            return output.ToString();
        }
        public static char[] reservedChars = { ' ', '!', '*', '\'', '(', ')', ';', ':', '@', '&',
                                                 '=', '+', '$', ',', '/', '?', '%', '#', '[', ']' };
    }

    public enum Beat { Minecraft, TChalo }
}
