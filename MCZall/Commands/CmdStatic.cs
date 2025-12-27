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
    public class CmdStatic : Command
    {
        public override string Name { get { return "static"; } }
        public override string Shortcut { get { return "t"; } }
        public override string Type { get { return "build"; } }
        public CmdStatic() { }
        public override void Use(Player p, string message)
        {
            p.staticCommands = !p.staticCommands;
            p.ClearBlockchange();
            p.BlockAction = 0;

            p.SendMessage("Static mode: &a" + p.staticCommands.ToString());

            try
            {
                if (message != "")
                {
                    if (message.IndexOf(' ') == -1)
                    {
                        if (p.group.CanExecute(all.Find(message)))
                            all.Find(message).Use(p, "");
                        else
                            p.SendMessage("Cannot use that command.");
                    }
                    else
                    {
                        if (p.group.CanExecute(all.Find(message.Split(' ')[0])))
                            all.Find(message.Split(' ')[0]).Use(p, message.Substring(message.IndexOf(' ') + 1));
                        else
                            p.SendMessage("Cannot use that command.");
                    }
                }
            }
            catch { p.SendMessage("Could not find specified command"); }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/static [command] - Makes every command a toggle.");
            p.SendMessage("If [command] is given, then that command is used");
        }
    }
}