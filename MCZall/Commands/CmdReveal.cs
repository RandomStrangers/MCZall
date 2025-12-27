namespace MCZall
{
    public class CmdReveal : Command
    {
        public override string Name { get { return "reveal"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdReveal() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            if (message.ToLower() == "all")
            {
                if (p.group.Permission < LevelPermission.Operator) { p.SendMessage("Reserved for OP+"); return; }

                foreach (Player who in Player.players)
                {
                    if (who.level == p.level)
                    {
                        foreach (Player pl in Player.players) if (who.level == pl.level && who != pl) who.SendDie(pl.id);
                        foreach (PlayerBot b in PlayerBot.playerbots) if (who.level == b.level) who.SendDie(b.id);
                        Player.GlobalDie(who, true);

                        who.SendMap();

                        ushort x = (ushort)((0.5 + who.level.spawnx) * 32);
                        ushort y = (ushort)((1 + who.level.spawny) * 32);
                        ushort z = (ushort)((0.5 + who.level.spawnz) * 32);

                        Player.GlobalSpawn(who, x, y, z, who.level.rotx, who.level.roty, true);

                        foreach (Player pl in Player.players)
                            if (pl.level == who.level && who != pl && !pl.hidden)
                                who.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);

                        foreach (PlayerBot b in PlayerBot.playerbots)
                            if (b.level == who.level)
                                who.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);

                        who.SendMessage("Map reloaded.");
                    }
                }
            }
            else
            {
                Player who = Player.Find(message);
                if (who == null) { p.SendMessage("Could not find player."); return; }
                else if (who.group.Permission > p.group.Permission && p != who) { p.SendMessage("Cannot reload the map of someone higher than you."); return; }

                foreach (Player pl in Player.players) if (who.level == pl.level && who != pl) who.SendDie(pl.id);
                foreach (PlayerBot b in PlayerBot.playerbots) if (who.level == b.level) who.SendDie(b.id);
                Player.GlobalDie(who, true);

                who.SendMap();

                ushort x = (ushort)((0.5 + who.level.spawnx) * 32);
                ushort y = (ushort)((1 + who.level.spawny) * 32);
                ushort z = (ushort)((0.5 + who.level.spawnz) * 32);

                Player.GlobalSpawn(who, x, y, z, who.level.rotx, who.level.roty, true);

                foreach (Player pl in Player.players)
                    if (pl.level == who.level && who != pl && !pl.hidden)
                        who.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);

                foreach (PlayerBot b in PlayerBot.playerbots)
                    if (b.level == who.level)
                        who.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);

                who.SendMessage("Map reloaded.");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/reveal <name> - Reveals the map for <name>.");
            p.SendMessage("Will reload the map for anyone. (incl. banned)");
        }
    }
}