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
    public class CmdAbort : Command
    {
        public override string Name { get { return "abort"; } }
        public override string Shortcut { get { return "a"; } }
        public override string Type { get { return "build"; } }
        public CmdAbort() { }
        public override void Use(Player p, string message)
        {
            p.ClearBlockchange(); p.painting = false;
            p.BlockAction = 0; //p.exitPortal = false;
            p.megaBoid = false; p.cmdTimer = false;
            p.staticCommands = false; p.deleteMode = false;
            p.ZoneCheck = false; p.modeType = 0;
            p.SendMessage("Every toggle or action was aborted.");
        }
        public override void Help(Player p)
        {
            p.SendMessage("/abort - Cancels an action.");
        }
    }
}