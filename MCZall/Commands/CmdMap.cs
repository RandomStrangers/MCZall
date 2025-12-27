namespace MCZall
{
    public class CmdMap : Command
    {
        public override string Name { get { return "map"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdMap() { }
        public override void Use(Player p, string message)
        {
            if (message == "") message = p.level.name;

            Level foundLevel;

            if (message.IndexOf(' ') == -1)
            {
                foundLevel = Level.Find(message);
                if (foundLevel == null)
                {
                    foundLevel = p.level;
                }
                else
                {
                    p.SendMessage("Theme: &b" + foundLevel.theme);
                    p.SendMessage("Finite mode: " + FoundCheck(foundLevel.finite));
                    p.SendMessage("Animal AI: " + FoundCheck(foundLevel.ai));
                    p.SendMessage("Edge water: " + FoundCheck(foundLevel.edgeWater));
                    p.SendMessage("Physics speed: &b" + foundLevel.speedPhysics);
                    p.SendMessage("Physics overload: &b" + foundLevel.overload);
                    p.SendMessage("Survival death: " + FoundCheck(foundLevel.Death) + "(Fall: " + foundLevel.fall + ", Drown: " + foundLevel.drown + ")");
                    p.SendMessage("Killer blocks: " + FoundCheck(foundLevel.Killer));
                    p.SendMessage("MOTD: &b" + foundLevel.motd);
                    p.SendMessage("Unload: " + FoundCheck(foundLevel.unload));
                    p.SendMessage("Auto physics: " + FoundCheck(foundLevel.rp));
                    p.SendMessage("Instant building: " + FoundCheck(foundLevel.Instant));
                    p.SendMessage("RP chat: " + FoundCheck(!foundLevel.worldChat));
                    return;
                }
            }
            else
            {
                foundLevel = Level.Find(message.Split(' ')[0]);

                if (foundLevel == null || message.Split(' ')[0].ToLower() == "ps" || message.Split(' ')[0].ToLower() == "rp") foundLevel = p.level;
                else message = message.Substring(message.IndexOf(' ') + 1);
            }

            if (p != null)
                if (p.group.Permission < LevelPermission.Operator) { p.SendMessage("Setting map options is reserved to OP+"); return; }

            string foundStart;
            if (message.IndexOf(' ') == -1) foundStart = message.ToLower();
            else foundStart = message.Split(' ')[0].ToLower();

            try
            {
                switch (foundStart)
                {
                    case "theme": foundLevel.theme = message.Substring(message.IndexOf(' ') + 1); Player.GlobalChatLevel(p, "Map theme: &b" + foundLevel.theme, false); break;
                    case "finite": foundLevel.finite = !foundLevel.finite; Player.GlobalChatLevel(p, "Finite mode: " + FoundCheck(foundLevel.finite), false); break;
                    case "ai": foundLevel.ai = !foundLevel.ai; Player.GlobalChatLevel(p, "Animal AI: " + FoundCheck(foundLevel.ai), false); break;
                    case "edge": foundLevel.edgeWater = !foundLevel.edgeWater; Player.GlobalChatLevel(p, "Edge water: " + FoundCheck(foundLevel.edgeWater), false); break;
                    case "ps":
                    case "physicspeed":
                        if (int.Parse(message.Split(' ')[1]) < 10) { p.SendMessage("Cannot go below 10"); return; }
                        foundLevel.speedPhysics = int.Parse(message.Split(' ')[1]);
                        Player.GlobalChatLevel(p, "Physics speed: &b" + foundLevel.speedPhysics, false);
                        break;
                    case "overload":
                        if (int.Parse(message.Split(' ')[1]) < 500) { p.SendMessage("Cannot go below 500 (default is 1500)"); return; }
                        if (p.group.Permission < LevelPermission.Admin && int.Parse(message.Split(' ')[1]) > 2500) { p.SendMessage("Only SuperOPs may set higher than 2500"); return; }
                        foundLevel.overload = int.Parse(message.Split(' ')[1]);
                        Player.GlobalChatLevel(p, "Physics overload: &b" + foundLevel.overload, false);
                        break;
                    case "motd":
                        if (message.Split(' ').Length == 1) foundLevel.motd = "ignore";
                        else foundLevel.motd = message.Substring(message.IndexOf(' ') + 1);
                        Player.GlobalChatLevel(p, "Map MOTD: &b" + foundLevel.motd, false);
                        break;
                    case "death": foundLevel.Death = !foundLevel.Death; Player.GlobalChatLevel(p, "Survival death: " + FoundCheck(foundLevel.Death), false); break;
                    case "killer": foundLevel.Killer = !foundLevel.Killer; Player.GlobalChatLevel(p, "Killer blocks: " + FoundCheck(foundLevel.Killer), false); break;
                    case "fall": foundLevel.fall = int.Parse(message.Split(' ')[1]); Player.GlobalChatLevel(p, "Fall distance: &b" + foundLevel.fall, false); break;
                    case "drown": foundLevel.drown = int.Parse(message.Split(' ')[1]) * 10; Player.GlobalChatLevel(p, "Drown time: &b" + (foundLevel.drown / 10), false); break;
                    case "unload": foundLevel.unload = !foundLevel.unload; Player.GlobalChatLevel(p, "Auto unload: " + FoundCheck(foundLevel.unload), false); break;
                    case "rp":
                    case "restartphysics": foundLevel.rp = !foundLevel.rp; Player.GlobalChatLevel(p, "Auto physics: " + FoundCheck(foundLevel.rp), false); break;
                    case "instant":
                        if (p.group.Permission < LevelPermission.Admin) { p.SendMessage("This is reserved for Super+"); return; }
                        foundLevel.Instant = !foundLevel.Instant; Player.GlobalChatLevel(p, "Instant building: " + FoundCheck(foundLevel.Instant), false); break;
                    case "chat":
                        foundLevel.worldChat = !foundLevel.worldChat; Player.GlobalChatLevel(p, "RP chat: " + FoundCheck(!foundLevel.worldChat), false); break;
                    default: p.SendMessage("Could not find option entered."); return;
                }
                foundLevel.Save(true);
            }
            catch { p.SendMessage("INVALID INPUT"); }
        }
        public string FoundCheck(bool check)
        {
            if (check) return "&aON";
            else return "&cOFF";
        }

        public override void Help(Player p)
        {
            p.SendMessage("/map [level] [toggle] - Sets [toggle] on [map]");
            p.SendMessage("Possible toggles: theme, finite, ai, edge, ps, overload, motd, death, fall, drown, unload, rp, instant, killer, chat");
            p.SendMessage("Theme will set the map's theme.");
            p.SendMessage("Edge will cause edge water to flow.");
            p.SendMessage("Finite will cause all liquids to be finite.");
            p.SendMessage("AI will make animals hunt or flee.");
            p.SendMessage("PS will set the map's physics speed.");
            p.SendMessage("Overload will change how easy/hard it is to kill physics.");
            p.SendMessage("MOTD will set a custom motd for the map. (leave blank to reset)");
            p.SendMessage("Death will allow survival-style dying (falling, drowning)");
            p.SendMessage("Fall/drown set the distance/time before dying from each.");
            p.SendMessage("Killer turns killer blocks on and off.");
            p.SendMessage("Unload sets whether the map unloads when no one's there.");
            p.SendMessage("RP sets whether the physics auto-start for the map");
            p.SendMessage("Instant mode works by not updating everyone's screens");
            p.SendMessage("Chat sets the map to recieve no messages from other maps");
        }
    }
}