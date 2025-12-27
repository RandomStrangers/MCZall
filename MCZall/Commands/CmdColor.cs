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
    public class CmdColor : Command
    {
        public override string Name { get { return "color"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdColor() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (message.Split(' ').Length > 2) { Help(p); return; }
            int pos = message.IndexOf(' ');
            if (pos != -1)
            {
                Player who = Player.Find(message.Substring(0, pos));
                if (who == null) { p.SendMessage("There is no player \"" + message.Substring(0, pos) + "\"!"); return; }
                string color = C.Parse(message.Substring(pos + 1));
                if (color == "") { p.SendMessage("There is no color \"" + message + "\"."); }
                else if (color == who.color) { p.SendMessage(who.name + " already has that color."); }
                else
                {
                    //Player.GlobalChat(who, p.color + "*" + p.name + "&e changed " + who.color + Name(who.name) +
                    //                  " color to " + color +
                    //                  c.Name(color) + "&e.", false);
                    Player.GlobalChat(who, who.color + "*" + NameColor(who.name) + " color changed to " + color + C.Name(color) + Server.DefaultColor + ".", false);
                    who.color = color;

                    Player.GlobalDie(who, false);
                    Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
                }
            }
            else
            {
                string color = C.Parse(message);
                if (color == "") { p.SendMessage("There is no color \"" + message + "\"."); }
                else if (color == p.color) { p.SendMessage("You already have that color."); }
                else
                {
                    Player.GlobalChat(p, p.color + "*" + NameColor(p.name) +
                                      " color changed to " + color +
                                      C.Name(color) + Server.DefaultColor + ".", false);
                    p.color = color; Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/color [player] <color> - Changes the nick color.");
            p.SendChat(p, "&0black &1navy &2green &3teal &4maroon &5purple &6gold &7silver");
            p.SendChat(p, "&8gray &9blue &alime &baqua &cred &dpink &eyellow &fwhite");
        }
        static string NameColor(string name)
        {
            string ch = name[name.Length - 1].ToString().ToLower();
            if (ch == "s" || ch == "x") { return name + Server.DefaultColor + "'"; }
            else { return name + Server.DefaultColor + "'s"; }
        }
    }
}