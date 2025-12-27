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

namespace MCZall
{
    public class CmdTime : Command
    {
        public override string Name { get { return "time"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdTime() { }
        public override void Use(Player p, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            message = "Server time is " + time;
            p.SendMessage(message);
            //message = "full Date/Time is " + DateTime.Now.ToString();
            //p.SendMessage(message);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/time - Shows the server time.");
        }
    }
}