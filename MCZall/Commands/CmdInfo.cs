/*Q
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

namespace MCZall
{
    public class CmdInfo : Command
    {
        public override string Name { get { return "info"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdInfo() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); }
            else
            {
                p.SendMessage("This server runs on &bMCZall" + Server.DefaultColor + ", which started as MCSharp, and was made much more feature-packed by Zallist.");
                p.SendMessage("This server's version: &a" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                p.SendMessage("This server has been online for &b" + Convert.ToDateTime(DateTime.Now.Subtract(Server.timeOnline).ToString()).ToString("HH:mm:ss"));
                if (Server.updateTimer.Interval > 1000) p.SendMessage("Server is currently in &5Low Lag" + Server.DefaultColor + " mode.");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/info - Displays the server information.");
        }
    }
}
