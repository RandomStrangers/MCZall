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
namespace MCZall
{
    public class CmdUpdate : Command
    {
        public override string Name { get { return "update"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdUpdate() { }
        public override void Use(Player p, string message)
        {
            /*
            p.SendMessage("Please wait, checking for updates");

            Thread updateThread = new Thread(new ThreadStart(delegate {
                WebClient Client = new WebClient();
                try {
                    string foundString = Client.DownloadString("URLGOESHERE");
                    if (foundString != Server.Version) {
                        p.SendMessage("Server is out-of-date.");
                        p.SendMessage("Current version: &b" + Server.Version);
                        p.SendMessage("Latest version: &b" + foundString);
                    } else {
                        p.SendMessage("Server is up-to-date");
                    }
                } catch { p.SendMessage("Could not connect. Web server down."); }
                Client.Dispose();
            })); updateThread.Start();*/

            p.SendMessage("Disabled");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/update - Shows whether the server is out-of-date");
        }
    }
}