using MySql.Data.MySqlClient;
using System;

namespace MCZall
{
    public class CmdZone : Command
    {
        public override string Name { get { return "zone"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "mod"; } }
        public CmdZone() { }
        public override void Use(Player p, string message)
        {
            CatchPos cpos;

            if (message == "")
            {
                p.ZoneCheck = true;
                p.SendMessage("Place a block where you would like to check for zones.");
                return;
            }
            else if (p.group.Permission < LevelPermission.Operator)
            {
                p.SendMessage("Reserved for OP+");
                return;
            }

            if (message.IndexOf(' ') == -1)
            {
                switch (message.ToLower())
                {
                    case "del":
                        p.zoneDel = true;
                        p.SendMessage("Place a block where you would like to delete a zone.");
                        return;
                    default:
                        Help(p);
                        return;
                }
            }

            if (message.ToLower() == "del all")
            {
                if (p.group.Permission != LevelPermission.Admin)
                {
                    p.SendMessage("Only a SuperOP may delete all zones at once");
                    return;
                }

                int currentCount = 0;

                foreach (Level.Zone Zn in p.level.ZoneList)
                {
                    currentCount++;
                    p.level.ZoneList.Remove(Zn);
                }

                p.SendMessage("Deleted " + currentCount + " zones for this level");
                return;
            }

            if (p.group.Permission < LevelPermission.Operator)
            {
                p.SendMessage("Setting zones is reserved for OP+"); return;
            }

            if (Group.Find(message.Split(' ')[1]) != null)
            {
                message = message.Split(' ')[0] + " grp" + Group.Find(message.Split(' ')[1]).Name;
            }

            if (message.Split(' ')[0].ToLower() == "add")
            {
                Player foundPlayer = Player.Find(message.Split(' ')[1]);
                if (foundPlayer == null)
                    cpos.Owner = message.Split(' ')[1].ToString();
                else
                    cpos.Owner = foundPlayer.name;
            }
            else { Help(p); return; }

            if (!Player.ValidName(cpos.Owner)) { p.SendMessage("INVALID NAME."); return; }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            p.SendMessage("Place two blocks to determine the edges.");
            p.SendMessage("Zone for: &b" + cpos.Owner + ".");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/zone [add] [name] - Creates a zone only [name] can build in");
            p.SendMessage("/zone [add] [rank] - Creates a zone only [rank]+ can build in");
            p.SendMessage("/zone del - Deletes the zone clicked");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            Level.Zone Zn;

            Zn.smallX = Math.Min(cpos.x, x);
            Zn.smallY = Math.Min(cpos.y, y);
            Zn.smallZ = Math.Min(cpos.z, z);
            Zn.bigX = Math.Max(cpos.x, x);
            Zn.bigY = Math.Max(cpos.y, y);
            Zn.bigZ = Math.Max(cpos.z, z);
            Zn.Owner = cpos.Owner;

            p.level.ZoneList.Add(Zn);

            //DB
            MySqlCommand NewZone = Server.mysqlCon.CreateCommand(); int totalCount = 0;
            NewZone.CommandText = "INSERT INTO Zone" + p.level.name + " (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) VALUES (" + Zn.smallX + ", " + Zn.smallY + ", " + Zn.smallZ + ", " + Zn.bigX + ", " + Zn.bigY + ", " + Zn.bigZ + ", '" + Zn.Owner + "')";

        retryTag: try { NewZone.ExecuteNonQuery(); } catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }
            //DB

            p.SendMessage("Added zone for &b" + cpos.Owner);
        }

        struct CatchPos { public ushort x, y, z; public string Owner; }
    }
}