using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MCZall
{
    public class CmdPortal : Command
    {
        public override string Name { get { return "portal"; } }
        public override string Shortcut { get { return "o"; } }
        public override string Type { get { return "build"; } }
        public CmdPortal() { }
        public override void Use(Player p, string message)
        {
            PortalPos portalPos;

            portalPos.Multi = false;

            if (message.IndexOf(' ') != -1)
            {
                if (message.Split(' ')[1].ToLower() == "multi")
                {
                    portalPos.Multi = true;
                    message = message.Split(' ')[0];
                }
                else
                {
                    p.SendMessage("Invalid parameters");
                    return;
                }
            }

            if (message.ToLower() == "blue" || message == "") { portalPos.type = Block.blue_portal; }
            else if (message.ToLower() == "orange") { portalPos.type = Block.orange_portal; }
            else if (message.ToLower() == "air") { portalPos.type = Block.air_portal; }
            else if (message.ToLower() == "water") { portalPos.type = Block.water_portal; }
            else if (message.ToLower() == "lava") { portalPos.type = Block.lava_portal; }
            else if (message.ToLower() == "show") { ShowPortals(p); return; }
            else { Help(p); return; }

            p.ClearBlockchange();

            PortPos port;

            port.x = 0; port.y = 0; port.z = 0; port.portMap = null;
            portalPos.port = new List<PortPos>();

            p.blockchangeObject = portalPos;
            p.SendMessage("Place a the &aEntry block" + Server.DefaultColor + " for the portal"); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(EntryChange);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/portal [orange/blue/air/water/lava] [multi] - Activates Portal mode.");
            p.SendMessage("/portal [type] multi - Place Entry blocks until exit is wanted.");
            p.SendMessage("/portal show - Shows portals, green = in, red = out.");
        }

        public void EntryChange(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            PortalPos bp = (PortalPos)p.blockchangeObject;

            if (bp.Multi && type == Block.red && bp.port.Count > 0) { ExitChange(p, x, y, z, type); return; }

            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, Block.green);
            PortPos Port;

            Port.portMap = p.level;
            Port.x = x; Port.y = y; Port.z = z;

            bp.port.Add(Port);

            p.blockchangeObject = bp;

            if (!bp.Multi)
            {
                p.Blockchange += new Player.BlockchangeEventHandler(ExitChange);
                p.SendMessage("&aEntry block placed");
            }
            else
            {
                p.Blockchange += new Player.BlockchangeEventHandler(EntryChange);
                p.SendMessage("&aEntry block placed. &cRed block for exit");
            }
        }
        public void ExitChange(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            PortalPos bp = (PortalPos)p.blockchangeObject;

            foreach (PortPos pos in bp.port)
            {
                DataTable Portals = new DataTable("Portals");
                int totalCount = 0;
            retryTag: try
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Portals" + pos.portMap.name + " WHERE EntryX=" + (int)pos.x + " AND EntryY=" + (int)pos.y + " AND EntryZ=" + (int)pos.z, Server.mysqlCon))
                    { da.Fill(Portals); }
                }
                catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }
                Portals.Dispose();

                totalCount = 0;
                MySqlCommand NewPortal;
                if (Portals.Rows.Count == 0)
                {
                    NewPortal = p.playerCon.CreateCommand();
                    NewPortal.CommandText = "INSERT INTO Portals" + pos.portMap.name + " (EntryX, EntryY, EntryZ, ExitMap, ExitX, ExitY, ExitZ) VALUES (" + (int)pos.x + ", " + (int)pos.y + ", " + (int)pos.z + ", '" + p.level.name + "', " + (int)x + ", " + (int)y + ", " + (int)z + ")";
                retryTag1: try { NewPortal.ExecuteNonQuery(); } catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag1; else Server.ErrorLog(e); }
                }
                else
                {
                    NewPortal = p.playerCon.CreateCommand();
                    NewPortal.CommandText = "UPDATE Portals" + pos.portMap.name + " SET ExitMap='" + p.level.name + "', ExitX=" + (int)x + ", ExitY=" + (int)y + ", ExitZ=" + (int)z + " WHERE EntryX=" + (int)pos.x + " AND EntryY=" + (int)pos.y + " AND EntryZ=" + (int)pos.z;
                retryTag1: try { NewPortal.ExecuteNonQuery(); } catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag1; else Server.ErrorLog(e); }
                }
                //DB

                pos.portMap.Blockchange(p, pos.x, pos.y, pos.z, bp.type);
                NewPortal.Dispose();
            }

            p.SendBlockchange(x, y, z, Block.air);
            p.SendMessage(">&3Exit" + Server.DefaultColor + " block placed");

            if (p.staticCommands) { bp.port.Clear(); p.blockchangeObject = bp; p.Blockchange += new Player.BlockchangeEventHandler(EntryChange); }
        }

        public struct PortalPos { public List<PortPos> port; public byte type; public bool Multi; }
        public struct PortPos { public ushort x, y, z; public Level portMap; }

        public void ShowPortals(Player p)
        {
            p.showPortals = !p.showPortals;

            DataTable Portals = new DataTable("Portals"); int totalCount = 0;
        retryTag: try
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Portals" + p.level.name, Server.mysqlCon))
                { da.Fill(Portals); }
            }
            catch (Exception e) { totalCount++; if (totalCount < 10) goto retryTag; else Server.ErrorLog(e); }

            int i;

            if (p.showPortals)
            {
                for (i = 0; i < Portals.Rows.Count; i++)
                {
                    if (Portals.Rows[i]["ExitMap"].ToString() == p.level.name)
                        p.SendBlockchange((ushort)Portals.Rows[i]["ExitX"], (ushort)Portals.Rows[i]["ExitY"], (ushort)Portals.Rows[i]["ExitZ"], Block.orange_portal);
                    p.SendBlockchange((ushort)Portals.Rows[i]["EntryX"], (ushort)Portals.Rows[i]["EntryY"], (ushort)Portals.Rows[i]["EntryZ"], Block.blue_portal);
                }

                p.SendMessage("Now showing &a" + i.ToString() + Server.DefaultColor + " portals.");
            }
            else
            {
                for (i = 0; i < Portals.Rows.Count; i++)
                {
                    if (Portals.Rows[i]["ExitMap"].ToString() == p.level.name)
                        p.SendBlockchange((ushort)Portals.Rows[i]["ExitX"], (ushort)Portals.Rows[i]["ExitY"], (ushort)Portals.Rows[i]["ExitZ"], Block.air);

                    p.SendBlockchange((ushort)Portals.Rows[i]["EntryX"], (ushort)Portals.Rows[i]["EntryY"], (ushort)Portals.Rows[i]["EntryZ"], p.level.GetTile((ushort)Portals.Rows[i]["EntryX"], (ushort)Portals.Rows[i]["EntryY"], (ushort)Portals.Rows[i]["EntryZ"]));
                }

                p.SendMessage("Now hiding portals.");
            }

            Portals.Dispose();
        }
    }
}