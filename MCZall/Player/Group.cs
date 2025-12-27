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
using System.IO;

namespace MCZall
{
    public abstract class Group
    {
        public abstract string Name { get; }
        public abstract string Color { get; }
        public abstract bool CanChat { get; }
        //public abstract bool canBuild { get; }
        public abstract LevelPermission Permission { get; }
        public abstract int MaxBlocks { get; }
        public abstract CommandList Commands { get; }
        public bool CanExecute(Command cmd) { return Commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        public static Group standard;
        public static void InitAll()
        {
            GroupList = new List<Group>
            {
                new GrpBot(),
                new GrpBanned(),
                new GrpGuest(),
                new GrpBuilder(),
                new GrpAdvBuilder(),
                new GrpOperator(),
                new GrpSuperOp()
            };
            if (standard == null) standard = Find("guest"); //standard = GroupList[2];
        }
        public static bool Exists(string name)
        {
            name = name.ToLower();
            foreach (Group gr in GroupList)
            {
                if (gr.Name == name.ToLower()) { return true; }
            }
            return false;
        }
        public static Group Find(string name)
        {
            name = name.ToLower();

            if (name == "adv") name = "advbuilder";
            if (name == "op") name = "operator";
            if (name == "super" || name == "admin") name = "superop";

            foreach (Group gr in GroupList)
            {
                if (gr.Name == name.ToLower()) { return gr; }
            }
            return null;
        }
    }

    class GrpBot : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Admin; } }
        public override int MaxBlocks { get { return 1; } }
        public override string Name { get { return "bots"; } }
        public override string Color { get { return "[bot]&6"; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return false; } }
        readonly CommandList _commands = new CommandList();
        public override CommandList Commands { get { return _commands; } }
        public GrpBot() { }
    }

    public class GrpBanned : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Guest; } }
        public override int MaxBlocks { get { return 1; } }
        public override string Name { get { return "banned"; } }
        public override string Color { get { return Server.colBanned; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return false; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpBanned() { GrpCommands.AddCommands(out _commands, Permission); }
    }

    public class GrpGuest : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Guest; } }
        public override int MaxBlocks { get { return 1; } }
        public override string Name { get { return "guest"; } }
        public override string Color { get { return Server.colGuest; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return false; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpGuest() { GrpCommands.AddCommands(out _commands, Permission); }
    }

    public class GrpBuilder : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Builder; } }
        public override int MaxBlocks { get { return Server.maxBuild; } }
        public override string Name { get { return "builder"; } }
        public override string Color { get { return Server.colBuild; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return true; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpBuilder() { GrpCommands.AddCommands(out _commands, Permission); }
    }
    public class GrpAdvBuilder : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.AdvBuilder; } }
        public override int MaxBlocks { get { return Server.maxAdv; } }
        public override string Name { get { return "advbuilder"; } }
        public override string Color { get { return Server.colAdv; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return true; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpAdvBuilder() { GrpCommands.AddCommands(out _commands, Permission); }
    }
    public class GrpOperator : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Operator; } }
        public override int MaxBlocks { get { return Server.maxOp; } }
        public override string Name { get { return "operator"; } }
        public override string Color { get { return Server.colOp; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return true; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpOperator() { GrpCommands.AddCommands(out _commands, Permission); }
    }

    public class GrpSuperOp : Group
    {
        public override LevelPermission Permission { get { return LevelPermission.Admin; } }
        public override int MaxBlocks { get { return Server.maxSuper; } }
        public override string Name { get { return "superop"; } }
        public override string Color { get { return Server.colSuper; } }
        public override bool CanChat { get { return true; } }
        //public override bool canBuild { get { return true; } }
        public override CommandList Commands { get { return _commands; } }
        readonly CommandList _commands = new CommandList();
        public GrpSuperOp() { GrpCommands.AddCommands(out _commands, Permission); }
    }

    public class GrpCommands
    {
        public struct RankAllowance { public string commandName; public LevelPermission rank; }
        public static List<RankAllowance> allowedCommands;
        public static List<string> foundCommands = new List<string>();

        public static LevelPermission DefaultRanks(string command)
        {
            switch (command)
            {
                case "admins":
                case "emote":
                case "help":
                case "info":
                case "mapinfo":
                case "me":
                case "move":
                case "ops":
                case "rules":
                case "spawn":
                case "time":
                case "whois":
                case "whowas":
                case "host":
                    return LevelPermission.Banned;

                case "about":
                case "afk":
                case "fly":
                case "goto":
                case "inbox":
                case "levels":
                case "map":
                case "museum":
                case "mode":
                case "shortcuts":
                case "place":
                case "players":
                case "redo":
                case "repeat":
                case "ride":
                case "whisper":
                case "undo":
                case "unloaded":
                    return LevelPermission.Guest;

                case "abort":
                case "bots":
                case "cmdbind":
                case "cuboid":
                case "line":
                case "paint":
                case "send":
                case "spheroid":
                case "tp":
                case "tnt":
                case "tree":
                    return LevelPermission.Builder;

                case "bind":
                case "botai":
                case "botset":
                case "copy":
                case "fill":
                case "kick":
                case "mb":
                case "opchat":
                case "outline":
                case "paste":
                case "portal":
                case "replace":
                case "replacenot":
                case "restartphysics":
                case "spin":
                case "stairs":
                case "static":
                case "summon":
                case "reveal":
                case "clones":
                case "delete":
                case "update":
                    return LevelPermission.AdvBuilder;

                case "ban":
                case "banip":
                case "banned":
                case "color":
                case "freeze":
                case "follow":
                case "give":
                case "hide":
                case "invincible":
                case "jail":
                case "kickban":
                case "load":
                case "lowlag":
                case "megaboid":
                case "mute":
                case "pause":
                case "perbuild":
                case "pervisit":
                case "physics":
                case "restore":
                case "resetbot":
                case "roll":
                case "save":
                case "say":
                case "setrank":
                case "setspawn":
                //case "solid": 
                case "timer":
                case "unban":
                case "unbanip":
                case "unload":
                case "zone":
                    return LevelPermission.Operator;

                case "limit":
                case "deletelvl":
                case "newlvl":
                //case "nick": 
                case "serverreport":
                case "title":
                case "renamelvl":
                case "replaceall":
                case "botadd":
                case "botremove":
                case "botsummon":
                case "clearblockchanges":
                    return LevelPermission.Admin;

                default: return LevelPermission.Null;
            }
        }

        public static void FillRanks()
        {
            foundCommands = Command.all.CommandNames();
            allowedCommands = new List<RankAllowance>();

            RankAllowance allowVar;

            foreach (string cmd in foundCommands)
            {
                allowVar.commandName = cmd;
                allowVar.rank = DefaultRanks(cmd);
                allowedCommands.Add(allowVar);
            }

            if (File.Exists("properties/command.properties"))
            {
                string[] lines = File.ReadAllLines("properties/command.properties");

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim().ToLower();
                        string value = line.Split('=')[1].Trim().ToLower();

                        if (!foundCommands.Contains(key))
                        {
                            Server.s.Log("Incorrect command name: " + key);
                        }
                        else if (Level.PermissionFromName(value) == LevelPermission.Null)
                        {
                            Server.s.Log("Incorrect value given for " + key + ", using default value.");
                        }
                        else
                        {
                            allowVar.commandName = key;
                            allowVar.rank = Level.PermissionFromName(value);

                            int current = 0;
                            foreach (RankAllowance aV in allowedCommands)
                            {
                                if (key == aV.commandName) { allowedCommands[current] = allowVar; break; }
                                current++;
                            }
                        }
                    }
                }
                Server.s.Log("LOADED: command.properties");
                Save();
            }
            else Save();
        }

        public static void Save()
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create("properties/command.properties"));
                w.WriteLine("#   This file contains a reference to every command found in the server software");
                w.WriteLine("#   Use this file to specify which ranks get which commands");
                w.WriteLine("#   Current ranks: Banned, Guest, Builder, AdvBuilder, Operator, Admin (SuperOP)");
                w.WriteLine("");
                foreach (RankAllowance aV in allowedCommands)
                {
                    w.WriteLine(aV.commandName + " = " + Level.PermissionToName(aV.rank));
                }
                w.Flush();
                w.Close();

                Server.s.Log("SAVED: command.properties");
            }
            catch
            {
                Server.s.Log("SAVE FAILED! command.properties");
            }
        }

        public static void AddCommands(out CommandList commands, LevelPermission perm)
        {
            commands = new CommandList();

            foreach (RankAllowance aV in allowedCommands)
                if (aV.rank <= perm) commands.Add(Command.all.Find(aV.commandName));
        }
    }
}