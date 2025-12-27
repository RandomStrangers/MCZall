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
    public class CmdFly : Command
    {
        public override string Name { get { return "fly"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdFly() { }
        public override void Use(Player p, string message)
        {
            p.isFlying = !p.isFlying;

            if (message.ToLower() == "glass") p.flyGlass = true;
            else if (message.ToLower() == "water") p.flyGlass = false;

            p.SendMessage("Fly mode: &a" + p.isFlying.ToString());
            p.SendMessage("Glass mode: &a" + p.flyGlass.ToString());
        }
        public override void Help(Player p)
        {
            p.SendMessage("/fly [glass/water] - Places blocks under your feet.");
        }
    }
}