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
using System.Collections.Generic;

namespace MCZall
{
    public sealed class CommandList
    {
        public List<Command> commands = new List<Command>();
        public CommandList() { }
        public void Add(Command cmd) { commands.Add(cmd); }
        public void AddRange(List<Command> listCommands)
        {
            listCommands.ForEach(delegate (Command cmd) { commands.Add(cmd); });
        }
        public List<string> CommandNames()
        {
            List<string> tempList = new List<string>();

            commands.ForEach(delegate (Command cmd)
            {
                tempList.Add(cmd.Name);
            });

            return tempList;
        }

        public bool Remove(Command cmd) { return commands.Remove(cmd); }
        public bool Contains(Command cmd) { return commands.Contains(cmd); }
        public bool Contains(string name)
        {
            name = name.ToLower(); foreach (Command cmd in commands)
            {
                if (cmd.Name == name.ToLower()) { return true; }
            }
            return false;
        }
        public Command Find(string name)
        {
            name = name.ToLower(); foreach (Command cmd in commands)
            {
                if (cmd.Name == name.ToLower() || cmd.Shortcut == name.ToLower()) { return cmd; }
            }
            return null;
        }

        public string FindShort(string shortcut)
        {
            if (shortcut == "") return "";

            shortcut = shortcut.ToLower();
            foreach (Command cmd in commands)
            {
                if (cmd.Shortcut == shortcut) return cmd.Name;
            }
            return "";
        }

        public List<Command> All() { return new List<Command>(commands); }
    }
}