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
    public class CmdAdmins : Command
    {
        public override string Name { get { return "admins"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdAdmins() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            if (Server.operators.All().Count > 0)
            {
                Server.superOps.All().ForEach(delegate (string name) { message += ", " + name; });
                p.SendMessage(Server.superOps.All().Count + Group.Find("superop").Color + " Admin" + ((Server.superOps.All().Count != 1) ? "s" : "") + "&e: " + message.Remove(0, 2) + ".");
            }
            else { p.SendMessage("Nobody is admin. What's wrong with this server?"); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/admins - List the admins of the server");
        }
    }
}
