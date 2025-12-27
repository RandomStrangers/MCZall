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
    public class CmdOps : Command
    {
        public override string Name { get { return "ops"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "information"; } }
        public CmdOps() { }
        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            if (Server.operators.All().Count > 0)
            {
                Server.operators.All().ForEach(delegate (string name) { message += ", " + name; });
                p.SendMessage(Server.operators.All().Count + Group.Find("op").Color + " operator" + ((Server.operators.All().Count != 1) ? "s" : "") + Server.DefaultColor + ": " + message.Remove(0, 2) + ".");
            }
            else { p.SendMessage("Nobody is operator."); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/ops - Lists all operators.");
        }
    }
}