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
    public class Block
    {
        public const byte air = 0;
        public const byte rock = 1;
        public const byte grass = 2;
        public const byte dirt = 3;
        public const byte stone = 4;
        public const byte wood = 5;
        public const byte shrub = 6;
        public const byte blackrock = 7;// adminium
        public const byte water = 8;
        public const byte waterstill = 9;
        public const byte lava = 10;
        public const byte lavastill = 11;
        public const byte sand = 12;
        public const byte gravel = 13;
        public const byte goldrock = 14;
        public const byte ironrock = 15;
        public const byte coal = 16;
        public const byte trunk = 17;
        public const byte leaf = 18;
        public const byte sponge = 19;
        public const byte glass = 20;
        public const byte red = 21;
        public const byte orange = 22;
        public const byte yellow = 23;
        public const byte lightgreen = 24;
        public const byte green = 25;
        public const byte aquagreen = 26;
        public const byte cyan = 27;
        public const byte lightblue = 28;
        public const byte blue = 29;
        public const byte purple = 30;
        public const byte lightpurple = 31;
        public const byte pink = 32;
        public const byte darkpink = 33;
        public const byte darkgrey = 34;
        public const byte lightgrey = 35;
        public const byte white = 36;
        public const byte yellowflower = 37;
        public const byte redflower = 38;
        public const byte mushroom = 39;
        public const byte redmushroom = 40;
        public const byte goldsolid = 41;
        public const byte iron = 42;
        public const byte staircasefull = 43;
        public const byte staircasestep = 44;
        public const byte brick = 45;
        public const byte tnt = 46;
        public const byte bookcase = 47;
        public const byte stonevine = 48;
        public const byte obsidian = 49;
        public const byte Zero = 0xff;

        //Custom blocks
        public const byte op_glass = 100;
        public const byte opsidian = 101;
        public const byte op_brick = 102;
        public const byte op_stone = 103;
        public const byte op_cobblestone = 104;
        public const byte op_air = 105;
        public const byte op_water = 106;

        public const byte wood_float = 110;
        public const byte door = 111;
        public const byte lava_fast = 112;
        public const byte door2 = 113;
        public const byte door3 = 114;
        public const byte door4 = 115;
        public const byte door5 = 116;
        public const byte door6 = 117;
        public const byte door7 = 118;
        public const byte door8 = 119;
        public const byte door9 = 120;
        public const byte door10 = 121;

        public const byte tdoor = 122;
        public const byte tdoor2 = 123;
        public const byte tdoor3 = 124;
        public const byte tdoor4 = 125;
        public const byte tdoor5 = 126;
        public const byte tdoor6 = 127;
        public const byte tdoor7 = 128;
        public const byte tdoor8 = 129;

        //Messages
        public const byte MsgWhite = 130;
        public const byte MsgBlack = 131;
        public const byte MsgAir = 132;
        public const byte MsgWater = 133;
        public const byte MsgLava = 134;

        public const byte tdoor9 = 135;
        public const byte tdoor10 = 136;
        public const byte tdoor11 = 137;
        public const byte tdoor12 = 138;
        public const byte tdoor13 = 139;

        //"finite"
        public const byte WaterDown = 140;
        public const byte LavaDown = 141;
        public const byte WaterFaucet = 143;
        public const byte LavaFaucet = 144;

        public const byte finiteWater = 145;
        public const byte finiteLava = 146;
        public const byte finiteFaucet = 147;

        public const byte odoor1 = 148;
        public const byte odoor2 = 149;
        public const byte odoor3 = 150;
        public const byte odoor4 = 151;
        public const byte odoor5 = 152;
        public const byte odoor6 = 153;
        public const byte odoor7 = 154;
        public const byte odoor8 = 155;
        public const byte odoor9 = 156;
        public const byte odoor10 = 157;
        public const byte odoor11 = 158;
        public const byte odoor12 = 159;

        //movement
        public const byte air_portal = 160;
        public const byte water_portal = 161;
        public const byte lava_portal = 162;

        //Movement doors
        public const byte air_door = 164;
        public const byte air_switch = 165;
        public const byte water_door = 166;
        public const byte lava_door = 167;

        public const byte odoor1_air = 168;
        public const byte odoor2_air = 169;
        public const byte odoor3_air = 170;
        public const byte odoor4_air = 171;
        public const byte odoor5_air = 172;
        public const byte odoor6_air = 173;
        public const byte odoor7_air = 174;

        //portals
        public const byte blue_portal = 175;
        public const byte orange_portal = 176;

        public const byte odoor8_air = 177;
        public const byte odoor9_air = 178;
        public const byte odoor10_air = 179;
        public const byte odoor11_air = 180;
        public const byte odoor12_air = 181;

        //Explosions
        public const byte smalltnt = 182;
        public const byte bigtnt = 183;
        public const byte tntexplosion = 184;

        public const byte fire = 185;

        public const byte rocketstart = 187;
        public const byte rockethead = 188;
        public const byte firework = 189;

        //Death
        public const byte deathlava = 190;
        public const byte deathwater = 191;
        public const byte deathair = 192;

        public const byte activedeathwater = 193;
        public const byte activedeathlava = 194;

        public const byte magma = 195;
        public const byte geyser = 196;

        public const byte air_flood = 200;
        public const byte door_air = 201;
        public const byte air_flood_layer = 202;
        public const byte air_flood_down = 203;
        public const byte air_flood_up = 204;
        public const byte door2_air = 205;
        public const byte door3_air = 206;
        public const byte door4_air = 207;
        public const byte door5_air = 208;
        public const byte door6_air = 209;
        public const byte door7_air = 210;
        public const byte door8_air = 211;
        public const byte door9_air = 212;
        public const byte door10_air = 213;
        public const byte door11_air = 214;
        public const byte door12_air = 215;
        public const byte door13_air = 216;
        public const byte door14_air = 217;

        public const byte train = 230;

        public const byte creeper = 231;
        public const byte zombiebody = 232;
        public const byte zombiehead = 233;

        public const byte birdwhite = 235;
        public const byte birdblack = 236;
        public const byte birdwater = 237;
        public const byte birdlava = 238;
        public const byte birdred = 239;
        public const byte birdblue = 240;
        public const byte birdkill = 242;

        public const byte fishgold = 245;
        public const byte fishsponge = 246;
        public const byte fishshark = 247;
        public const byte fishsalmon = 248;
        public const byte fishbetta = 249;

        public static List<Blocks> BlockList = new List<Blocks>();
        public struct Blocks { public byte type; public LevelPermission placable; }

        public static void SetBlocks()
        {
            Blocks b;
            b.placable = LevelPermission.Guest;

            for (int i = 0; i < 256; i++)
            {
                b.type = (byte)i;
                BlockList.Add(b);
            }

            List<Blocks> storedList = new List<Blocks>();

            foreach (Blocks bs in BlockList)
            {
                b.type = bs.type;

                switch (bs.type)
                {
                    case Zero:
                        b.placable = LevelPermission.Admin;
                        break;


                    case op_glass:
                    case opsidian:
                    case op_brick:
                    case op_stone:
                    case op_cobblestone:
                    case op_air:
                    case op_water:
                    case blackrock:

                    case air_flood:
                    case air_flood_down:
                    case air_flood_layer:
                    case air_flood_up:

                    case rocketstart:
                    case rockethead:

                    case creeper:
                    case zombiebody:
                    case zombiehead:

                    case birdred:
                    case birdkill:
                    case birdblue:

                    case fishgold:
                    case fishsponge:
                    case fishshark:
                    case fishsalmon:
                    case fishbetta:

                        b.placable = LevelPermission.Operator;
                        break;


                    case wood_float:

                    case door_air:
                    case door2_air:
                    case door3_air:
                    case door4_air:
                    case door5_air:
                    case door6_air:
                    case door7_air:
                    case door8_air:
                    case door9_air:
                    case door10_air:
                    case door11_air:
                    case door12_air:
                    case door13_air:
                    case door14_air:

                    case odoor1_air:
                    case odoor2_air:
                    case odoor3_air:
                    case odoor4_air:
                    case odoor5_air:
                    case odoor6_air:
                    case odoor7_air:
                    case odoor8_air:
                    case odoor9_air:
                    case odoor10_air:
                    case odoor11_air:
                    case odoor12_air:

                    case MsgAir:
                    case MsgBlack:
                    case MsgLava:
                    case MsgWater:
                    case MsgWhite:
                    case air_portal:
                    case water_portal:
                    case lava_portal:
                    case blue_portal:
                    case orange_portal:

                    case water:
                    case lava:
                    case lava_fast:
                    case WaterDown:
                    case LavaDown:
                    case WaterFaucet:
                    case LavaFaucet:
                    case finiteWater:
                    case finiteLava:
                    case finiteFaucet:
                    case magma:
                    case geyser:
                    case deathlava:
                    case deathwater:
                    case deathair:
                    case activedeathwater:
                    case activedeathlava:
                    case fire:

                    case smalltnt:
                    case firework:

                    case train:

                    case birdwhite:
                    case birdblack:
                    case birdwater:
                    case birdlava:
                        b.placable = LevelPermission.AdvBuilder;
                        break;

                    case door:
                    case door2:
                    case door3:
                    case door4:
                    case door5:
                    case door6:
                    case door7:
                    case door8:
                    case door9:
                    case door10:
                    case air_door:
                    case air_switch:
                    case water_door:
                    case lava_door:

                    case tdoor:
                    case tdoor2:
                    case tdoor3:
                    case tdoor4:
                    case tdoor5:
                    case tdoor6:
                    case tdoor7:
                    case tdoor8:
                    case tdoor9:
                    case tdoor10:
                    case tdoor11:
                    case tdoor12:
                    case tdoor13:

                    case odoor1:
                    case odoor2:
                    case odoor3:
                    case odoor4:
                    case odoor5:
                    case odoor6:
                    case odoor7:
                    case odoor8:
                    case odoor9:
                    case odoor10:
                    case odoor11:
                    case odoor12:

                        b.placable = LevelPermission.Builder;
                        break;

                    default:
                        b.placable = LevelPermission.Banned;
                        break;
                }

                storedList.Add(b);
            }

            BlockList = storedList;
        }

        public static LevelPermission AllowPlace(byte type)
        {
            foreach (Blocks b in BlockList)
            {
                if (b.type == type) return b.placable;
            }

            return LevelPermission.Null;
        }

        public static bool Walkthrough(byte type)
        {
            switch (type)
            {
                case air:
                case water:
                case waterstill:
                case lava:
                case lavastill:
                case yellowflower:
                case redflower:
                case mushroom:
                case redmushroom:
                case shrub:
                    return true;
            }
            return false;
        }

        public static bool AnyBuild(byte type)
        {
            switch (type)
            {
                case air:
                case rock:
                case grass:
                case dirt:
                case stone:
                case wood:
                case shrub:
                case sand:
                case gravel:
                case goldrock:
                case ironrock:
                case coal:
                case trunk:
                case leaf:
                case sponge:
                case glass:
                case red:
                case orange:
                case yellow:
                case lightgreen:
                case green:
                case aquagreen:
                case cyan:
                case lightblue:
                case blue:
                case purple:
                case lightpurple:
                case pink:
                case darkpink:
                case darkgrey:
                case lightgrey:
                case white:
                case yellowflower:
                case redflower:
                case mushroom:
                case redmushroom:
                case goldsolid:
                case iron:
                case staircasefull:
                case staircasestep:
                case brick:
                case tnt:
                case bookcase:
                case stonevine:
                case obsidian:
                    return true;
            }
            return false;
        }

        public static bool AllowBreak(byte type)
        {
            switch (type)
            {
                case blue_portal:
                case orange_portal:

                case MsgWhite:
                case MsgBlack:

                case door:
                case door2:
                case door3:
                case door4:
                case door5:
                case door6:
                case door7:
                case door8:
                case door9:
                case door10:

                case smalltnt:
                case bigtnt:
                case rocketstart:
                case firework:

                case zombiebody:
                case creeper:
                case zombiehead:
                    return true;
            }
            return false;
        }

        public static bool Placable(byte type)
        {
            switch (type)
            {
                //				case Block.air:
                //				case Block.grass:
                case blackrock:
                case water:
                case waterstill:
                case lava:
                case lavastill:
                    return false;
            }

            if (type > 49) { return false; }
            return true;
        }

        public static bool OPBlocks(byte type)
        {
            switch (type)
            {
                case blackrock:
                case op_air:
                case op_brick:
                case op_cobblestone:
                case op_glass:
                case op_stone:
                case op_water:
                case opsidian:
                case rocketstart:

                case Zero:
                    return true;
            }
            return false;
        }

        public static bool Death(byte type)
        {
            switch (type)
            {
                case tntexplosion:

                case deathwater:
                case deathlava:
                case deathair:
                case activedeathlava:
                case activedeathwater:

                case magma:
                case geyser:

                case birdkill:
                case fishshark:

                case train:

                case fire:
                case rockethead:

                case creeper:
                case zombiebody:
                    //case zombiehead:
                    return true;
            }
            return false;
        }

        public static bool BuildIn(byte type)
        {
            switch (type)
            {
                case water:
                case lava:
                case waterstill:
                case lavastill:
                case finiteWater:
                case finiteLava:
                    return true;
            }
            return false;
        }

        public static bool Mover(byte type)
        {
            switch (type)
            {
                case air_portal:
                case water_portal:
                case lava_portal:

                case air_switch:
                case water_door:
                case lava_door:

                case MsgAir:
                case MsgWater:
                case MsgLava:
                    return true;
            }
            return false;
        }

        public static bool LavaKill(byte type)
        {
            switch (type)
            {
                case wood:
                case shrub:
                case trunk:
                case leaf:
                case sponge:
                case red:
                case orange:
                case yellow:
                case lightgreen:
                case green:
                case aquagreen:
                case cyan:
                case lightblue:
                case blue:
                case purple:
                case lightpurple:
                case pink:
                case darkpink:
                case darkgrey:
                case lightgrey:
                case white:
                case yellowflower:
                case redflower:
                case mushroom:
                case redmushroom:
                case bookcase:
                    return true;
            }
            return false;
        }
        public static bool WaterKill(byte type)
        {
            switch (type)
            {
                case air:
                case shrub:
                case leaf:
                case yellowflower:
                case redflower:
                case mushroom:
                case redmushroom:
                    return true;
            }
            return false;
        }

        public static bool LightPass(byte type)
        {
            switch (Convert(type))
            {
                case air:
                case glass:
                case leaf:
                case redflower:
                case yellowflower:
                case mushroom:
                case redmushroom:
                case shrub:
                    return true;

                default:
                    return false;
            }
        }

        public static bool NeedRestart(byte type)
        {
            switch (type)
            {
                case train:

                case fire:
                case rockethead:
                case firework:

                case creeper:
                case zombiebody:
                case zombiehead:

                case birdblack:
                case birdblue:
                case birdkill:
                case birdlava:
                case birdred:
                case birdwater:
                case birdwhite:

                case fishbetta:
                case fishgold:
                case fishsalmon:
                case fishshark:
                case fishsponge:

                case tntexplosion:
                    return true;
            }
            return false;
        }

        public static bool Portal(byte type)
        {
            switch (type)
            {
                case blue_portal:
                case orange_portal:
                case air_portal:
                case water_portal:
                case lava_portal:
                    return true;
            }
            return false;
        }
        public static bool Mb(byte type)
        {
            switch (type)
            {
                case MsgAir:
                case MsgWater:
                case MsgLava:
                case MsgBlack:
                case MsgWhite:
                    return true;
            }
            return false;
        }

        public static bool Physics(byte type)   //returns false if placing block cant actualy cause any physics to happen
        {
            switch (type)
            {
                case rock:
                case stone:
                case blackrock:
                case waterstill:
                case lavastill:
                case goldrock:
                case ironrock:
                case coal:
                case red:
                case orange:
                case yellow:
                case lightgreen:
                case green:
                case aquagreen:
                case cyan:
                case lightblue:
                case blue:
                case purple:
                case lightpurple:
                case pink:
                case darkpink:
                case darkgrey:
                case lightgrey:
                case white:
                case goldsolid:
                case iron:
                case staircasefull:
                case brick:
                case tnt:
                case stonevine:
                case obsidian:

                case op_glass:
                case opsidian:
                case op_brick:
                case op_stone:
                case op_cobblestone:
                case op_air:
                case op_water:

                case door:
                case door2:
                case door3:
                case door4:
                case door5:
                case door6:
                case door7:
                case door8:
                case door9:
                case door10:

                case tdoor:
                case tdoor2:
                case tdoor3:
                case tdoor4:
                case tdoor5:
                case tdoor6:
                case tdoor7:
                case tdoor8:
                case tdoor9:
                case tdoor10:
                case tdoor11:
                case tdoor12:
                case tdoor13:

                case air_door:
                case air_switch:
                case water_door:
                case lava_door:

                case MsgAir:
                case MsgWater:
                case MsgLava:
                case MsgBlack:
                case MsgWhite:

                case blue_portal:
                case orange_portal:
                case air_portal:
                case water_portal:
                case lava_portal:

                case deathair:
                case deathlava:
                case deathwater:
                    return false;

                default:
                    return true;
            }
        }

        public static string Name(byte type)
        {
            switch (type)
            {
                case 0: return "air";
                case 1: return "stone";
                case 2: return "grass";
                case 3: return "dirt";
                case 4: return "cobblestone";
                case 5: return "wood";
                case 6: return "plant";
                case 7: return "adminium";
                case 8: return "active_water";
                case 9: return "water";
                case 10: return "active_lava";
                case 11: return "lava";
                case 12: return "sand";
                case 13: return "gravel";
                case 14: return "gold_ore";
                case 15: return "iron_ore";
                case 16: return "coal";
                case 17: return "tree";
                case 18: return "leaves";
                case 19: return "sponge";
                case 20: return "glass";
                case 21: return "red";
                case 22: return "orange";
                case 23: return "yellow";
                case 24: return "greenyellow";
                case 25: return "green";
                case 26: return "springgreen";
                case 27: return "cyan";
                case 28: return "blue";
                case 29: return "blueviolet";
                case 30: return "indigo";
                case 31: return "purple";
                case 32: return "magenta";
                case 33: return "pink";
                case 34: return "black";
                case 35: return "gray";
                case 36: return "white";
                case 37: return "yellow_flower";
                case 38: return "red_flower";
                case 39: return "brown_shroom";
                case 40: return "red_shroom";
                case 41: return "gold";
                case 42: return "iron";
                case 43: return "double_stair";
                case 44: return "stair";
                case 45: return "brick";
                case 46: return "tnt";
                case 47: return "bookcase";
                case 48: return "mossy_cobblestone";
                case 49: return "obsidian";

                case 100: return "op_glass";
                case 101: return "opsidian";              //TODO Add command or just use bind?
                case 102: return "op_brick";              //TODO
                case 103: return "op_stone";              //TODO
                case 104: return "op_cobblestone";        //TODO
                case 105: return "op_air";                //TODO
                case 106: return "op_water";              //TODO

                case wood_float: return "wood_float";            //TODO
                case door: return "door_wood";
                case lava_fast: return "lava_fast";
                case door2: return "door_obsidian";
                case door3: return "door_glass";
                case door4: return "door_stone";
                case door5: return "door_leaves";
                case door6: return "door_sand";
                case door7: return "door_wood";
                case door8: return "door_green";
                case door9: return "door_tnt";
                case door10: return "door_stair";

                case tdoor: return "tdoor_wood";
                case tdoor2: return "tdoor_obsidian";
                case tdoor3: return "tdoor_glass";
                case tdoor4: return "tdoor_stone";
                case tdoor5: return "tdoor_leaves";
                case tdoor6: return "tdoor_sand";
                case tdoor7: return "tdoor_wood";
                case tdoor8: return "tdoor_green";
                case tdoor9: return "tdoor_tnt";
                case tdoor10: return "tdoor_stair";
                case tdoor11: return "tdoor_air";
                case tdoor12: return "tdoor_water";
                case tdoor13: return "tdoor_lava";

                case odoor1: return "odoor_wood";
                case odoor2: return "odoor_obsidian";
                case odoor3: return "odoor_glass";
                case odoor4: return "odoor_stone";
                case odoor5: return "odoor_leaves";
                case odoor6: return "odoor_sand";
                case odoor7: return "odoor_wood";
                case odoor8: return "odoor_green";
                case odoor9: return "odoor_tnt";
                case odoor10: return "odoor_stair";
                case odoor11: return "odoor_lava";
                case odoor12: return "odoor_water";

                case odoor1_air: return "odoor_wood_air";
                case odoor2_air: return "odoor_obsidian_air";
                case odoor3_air: return "odoor_glass_air";
                case odoor4_air: return "odoor_stone_air";
                case odoor5_air: return "odoor_leaves_air";
                case odoor6_air: return "odoor_sand_air";
                case odoor7_air: return "odoor_wood_air";
                case odoor8_air: return "odoor_red";
                case odoor9_air: return "odoor_tnt_air";
                case odoor10_air: return "odoor_stair_air";
                case odoor11_air: return "odoor_lava_air";
                case odoor12_air: return "odoor_water_air";

                case 130: return "white_message";
                case 131: return "black_message";
                case 132: return "air_message";
                case 133: return "water_message";
                case 134: return "lava_message";

                case 140: return "waterfall";
                case 141: return "lavafall";
                case WaterFaucet: return "water_faucet";
                case LavaFaucet: return "lava_faucet";

                case finiteWater: return "finite_water";
                case finiteLava: return "finite_lava";
                case finiteFaucet: return "finite_faucet";

                case 160: return "air_portal";
                case 161: return "water_portal";
                case 162: return "lava_portal";

                case air_door: return "air_door";
                case air_switch: return "air_switch";
                case water_door: return "door_water";
                case lava_door: return "door_lava";

                case 175: return "blue_portal";
                case 176: return "orange_portal";

                case 182: return "small_tnt";
                case 183: return "big_tnt";
                case 184: return "tnt_explosion";

                case fire: return "fire";

                case rocketstart: return "rocketstart";
                case rockethead: return "rockethead";
                case firework: return "firework";

                case 190: return "hot_lava";
                case 191: return "cold_water";
                case 192: return "nerve_gas";
                case activedeathwater: return "active_cold_water";
                case activedeathlava: return "active_hot_lava";

                case 195: return "magma";
                case 196: return "geyser";

                //Blocks after this are converted before saving
                case 200: return "air_flood";
                case 201: return "door_air";
                case 202: return "air_flood_layer";
                case 203: return "air_flood_down";
                case 204: return "air_flood_up";
                case 205: return "door2_air";
                case 206: return "door3_air";
                case 207: return "door4_air";
                case 208: return "door5_air";
                case 209: return "door6_air";
                case 210: return "door7_air";
                case 211: return "door8_air";
                case 212: return "door9_air";
                case 213: return "door10_air";
                case 214: return "door11_air";
                case 215: return "door12_air";
                case 216: return "door13_air";
                case 217: return "door14_air";

                //"AI" blocks
                case train: return "train";

                case creeper: return "creeper";
                case zombiebody: return "zombie";
                case zombiehead: return "zombie_head";

                case birdblue: return "blue_bird";
                case birdred: return "red_robin";
                case birdwhite: return "dove";
                case birdblack: return "pidgeon";
                case birdwater: return "duck";
                case birdlava: return "phoenix";
                case birdkill: return "killer_phoenix";

                case fishbetta: return "betta_fish";
                case fishgold: return "goldfish";
                case fishsalmon: return "salmon";
                case fishshark: return "shark";
                case fishsponge: return "sea_sponge";

                default: return "unknown";
            }
        }
        public static byte Byte(string type)
        {
            switch (type.ToLower())
            {
                case "air": return 0;
                case "stone": return 1;
                case "grass": return 2;
                case "dirt": return 3;
                case "cobblestone": return 4;
                case "wood": return 5;
                case "plant": return 6;
                case "solid": case "admintite": case "blackrock": case "adminium": return 7;
                case "activewater": case "active_water": return 8;
                case "water": return 9;
                case "activelava": case "active_lava": return 10;
                case "lava": return 11;
                case "sand": return 12;
                case "gravel": return 13;
                case "gold_ore": return 14;
                case "iron_ore": return 15;
                case "coal": return 16;
                case "tree": return 17;
                case "leaves": return 18;
                case "sponge": return 19;
                case "glass": return 20;
                case "red": return 21;
                case "orange": return 22;
                case "yellow": return 23;
                case "greenyellow": return 24;
                case "green": return 25;
                case "springgreen": return 26;
                case "cyan": return 27;
                case "blue": return 28;
                case "blueviolet": return 29;
                case "indigo": return 30;
                case "purple": return 31;
                case "magenta": return 32;
                case "pink": return 33;
                case "black": return 34;
                case "gray": return 35;
                case "white": return 36;
                case "yellow_flower": return 37;
                case "red_flower": return 38;
                case "brown_shroom": return 39;
                case "red_shroom": return 40;
                case "gold": return 41;
                case "iron": return 42;
                case "double_stair": return 43;
                case "stair": return 44;
                case "brick": return 45;
                case "tnt": return 46;
                case "bookcase": return 47;
                case "mossy_cobblestone": return 48;
                case "obsidian": return 49;

                case "op_glass": return 100;
                case "opsidian": return 101;              //TODO Add command or just use bind?
                case "op_brick": return 102;              //TODO
                case "op_stone": return 103;              //TODO
                case "op_cobblestone": return 104;        //TODO
                case "op_air": return 105;                //TODO
                case "op_water": return 106;              //TODO

                case "wood_float": return 110;            //TODO
                case "lava_fast": return 112;

                case "door_tree":
                case "door": return door;
                case "door_obsidian":
                case "door2": return door2;
                case "door_glass":
                case "door3": return door3;
                case "door_stone":
                case "door4": return door4;
                case "door_leaves":
                case "door5": return door5;
                case "door_sand":
                case "door6": return door6;
                case "door_wood":
                case "door7": return door7;
                case "door_green":
                case "door8": return door8;
                case "door_tnt":
                case "door9": return door9;
                case "door_stair":
                case "door10": return door10;

                case "tdoor_tree":
                case "tdoor": return tdoor;
                case "tdoor_obsidian":
                case "tdoor2": return tdoor2;
                case "tdoor_glass":
                case "tdoor3": return tdoor3;
                case "tdoor_stone":
                case "tdoor4": return tdoor4;
                case "tdoor_leaves":
                case "tdoor5": return tdoor5;
                case "tdoor_sand":
                case "tdoor6": return tdoor6;
                case "tdoor_wood":
                case "tdoor7": return tdoor7;
                case "tdoor_green":
                case "tdoor8": return tdoor8;
                case "tdoor_tnt":
                case "tdoor9": return tdoor9;
                case "tdoor_stair":
                case "tdoor10": return tdoor10;
                case "tair_switch":
                case "tdoor11": return tdoor11;
                case "tdoor_water":
                case "tdoor12": return tdoor12;
                case "tdoor_lava":
                case "tdoor13": return tdoor13;

                case "odoor_tree":
                case "odoor": return odoor1;
                case "odoor_obsidian":
                case "odoor2": return odoor2;
                case "odoor_glass":
                case "odoor3": return odoor3;
                case "odoor_stone":
                case "odoor4": return odoor4;
                case "odoor_leaves":
                case "odoor5": return odoor5;
                case "odoor_sand":
                case "odoor6": return odoor6;
                case "odoor_wood":
                case "odoor7": return odoor7;
                case "odoor_green":
                case "odoor8": return odoor8;
                case "odoor_tnt":
                case "odoor9": return odoor9;
                case "odoor_stair":
                case "odoor10": return odoor10;
                case "odoor_lava":
                case "odoor11": return odoor11;
                case "odoor_water":
                case "odoor12": return odoor12;
                case "odoor_red": return odoor8_air;

                case "white_message": return 130;
                case "black_message": return 131;
                case "air_message": return 132;
                case "water_message": return 133;
                case "lava_message": return 134;

                case "waterfall": return 140;
                case "lavafall": return 141;
                case "water_faucet": return WaterFaucet;
                case "lava_faucet": return LavaFaucet;

                case "finite_water": return finiteWater;
                case "finite_lava": return finiteLava;
                case "finite_faucet": return finiteFaucet;

                case "air_portal": return 160;
                case "water_portal": return 161;
                case "lava_portal": return 162;

                case "air_door": return air_door;
                case "air_switch": return air_switch;
                case "door_water": case "water_door": return water_door;
                case "door_lava": case "lava_door": return lava_door;

                case "blue_portal": return 175;
                case "orange_portal": return 176;

                case "fly": return 180;

                case "small_tnt": return 182;
                case "big_tnt": return 183;
                case "tnt_explosion": return 184;

                case "fire": return fire;

                case "rocketstart": return rocketstart;
                case "rockethead": return rockethead;
                case "firework": return firework;

                case "hot_lava": return 190;
                case "cold_water": return 191;
                case "nerve_gas": return 192;
                case "acw":
                case "active_cold_water": return activedeathwater;
                case "ahl":
                case "active_hot_lava": return activedeathlava;

                case "magma": return 195;
                case "geyser": return 196;

                //Blocks after this are converted before saving
                case "air_flood": return 200;
                //case "door_air": return 201;
                case "air_flood_layer": return 202;
                case "air_flood_down": return 203;
                case "air_flood_up": return 204;
                /*
            case "door2_air": return 205;
            case "door3_air": return 206;
            case "door4_air": return 207;
            case "door5_air": return 208;
            case "door6_air": return 209;
            case "door7_air": return 210;
            case "door8_air": return 211;
            case "door9_air": return 212;
            case "door10_air": return 213;
            case "door11_air": return 214;
            case "door12_air": return 215;
            case "door13_air": return 216;
            case "door14_air": return 217;*/

                case "train": return train;

                case "creeper": return creeper;
                case "zombie": return zombiebody;
                case "zombie_head": return zombiehead;

                case "blue_bird": return birdblue;
                case "red_robin": return birdred;
                case "dove": return birdwhite;
                case "pidgeon": return birdblack;
                case "duck": return birdwater;
                case "phoenix": return birdlava;
                case "killer_phoenix": return birdkill;

                case "betta_fish": return fishbetta;
                case "goldfish": return fishgold;
                case "salmon": return fishsalmon;
                case "shark": return fishshark;
                case "sea_sponge": return fishsponge;

                default: return Zero;
            }
        }

        public static byte Convert(byte b)
        {
            switch (b)
            {
                case 100: return 20; //Op_glass
                case 101: return 49; //Opsidian
                case 102: return 45; //Op_brick
                case 103: return 1; //Op_stone
                case 104: return 4; //Op_cobblestone
                case 105: return 0; //Op_air - Must be cuboided / replaced
                case 106: return waterstill; //Op_water

                case 110: return 5; //wood_float
                case 112: return 10;

                case door: return trunk;//door show by treetype
                case door2: return obsidian;//door show by obsidian
                case door3: return glass;//door show by glass
                case door4: return rock;//door show by stone
                case door5: return leaf;//door show by leaves
                case door6: return sand;//door show by sand
                case door7: return wood;//door show by wood
                case door8: return green;
                case door9: return tnt;//door show by TNT
                case door10: return staircasestep;//door show by Stair

                case tdoor: return trunk;//tdoor show by treetype
                case tdoor2: return obsidian;//tdoor show by obsidian
                case tdoor3: return glass;//tdoor show by glass
                case tdoor4: return rock;//tdoor show by stone
                case tdoor5: return leaf;//tdoor show by leaves
                case tdoor6: return sand;//tdoor show by sand
                case tdoor7: return wood;//tdoor show by wood
                case tdoor8: return green;
                case tdoor9: return tnt;//tdoor show by TNT
                case tdoor10: return staircasestep;//tdoor show by Stair
                case tdoor11: return air;
                case tdoor12: return waterstill;
                case tdoor13: return lavastill;

                case odoor1: return trunk;//odoor show by treetype
                case odoor2: return obsidian;//odoor show by obsidian
                case odoor3: return glass;//odoor show by glass
                case odoor4: return rock;//odoor show by stone
                case odoor5: return leaf;//odoor show by leaves
                case odoor6: return sand;//odoor show by sand
                case odoor7: return wood;//odoor show by wood
                case odoor8: return green;
                case odoor9: return tnt;//odoor show by TNT
                case odoor10: return staircasestep;//odoor show by Stair
                case odoor11: return lavastill;
                case odoor12: return waterstill;

                case 130: return 36;  //upVator
                case 131: return 34;  //upVator
                case 132: return 0;   //upVator
                case MsgWater: return waterstill;   //upVator
                case MsgLava: return lavastill;  //upVator

                case 140: return 8;
                case 141: return 10;
                case WaterFaucet: return cyan;
                case LavaFaucet: return orange;

                case finiteWater: return water;
                case finiteLava: return lava;
                case finiteFaucet: return lightblue;

                case 160: return 0;//air portal
                case 161: return waterstill;//water portal
                case 162: return lavastill;//lava portal

                case air_door: return air;
                case air_switch: return air;//air door
                case water_door: return waterstill;//water door
                case lava_door: return lavastill;

                case 175: return 28;//blue portal
                case 176: return 22;//orange portal

                case 182: return 46;//smalltnt
                case 183: return 46;//bigtnt
                case 184: return 10;//explosion

                case fire: return lava;

                case rocketstart: return glass;
                case rockethead: return goldsolid;
                case firework: return iron;

                case deathwater: return waterstill;
                case deathlava: return lavastill;
                case deathair: return 0;
                case activedeathwater: return water;
                case activedeathlava: return lava;

                case magma: return lava;
                case geyser: return water;

                case 200: //air_flood
                case 201: //door_air
                case 202: //air_flood_layer
                case 203: //air_flood_down
                case 204: //air_flood_up
                case 205: //door2_air
                case 206: //door3_air
                case 207: //door4_air
                case 208: //door5_air
                case 209: //door6_air
                case 210: //door7_air
                case 213: //door10_air
                case 214: //door10_air
                case 215: //door10_air
                case 216: //door10_air
                case door14_air:
                    return 0;
                case door9_air: return lava;
                case door8_air: return red;

                case odoor1_air:
                case odoor2_air:
                case odoor3_air:
                case odoor4_air:
                case odoor5_air:
                case odoor6_air:
                case odoor7_air:
                case odoor10_air:
                case odoor11_air:
                case odoor12_air:
                    return air;
                case odoor8_air: return red;
                case odoor9_air: return lavastill;

                case train: return cyan;

                case creeper: return tnt;
                case zombiebody: return stonevine;
                case zombiehead: return lightgreen;

                case birdwhite: return white;
                case birdblack: return darkgrey;
                case birdlava: return lava;
                case birdred: return red;
                case birdwater: return water;
                case birdblue: return blue;
                case birdkill: return lava;

                case fishbetta: return blue;
                case fishgold: return goldsolid;
                case fishsalmon: return red;
                case fishshark: return lightgrey;
                case fishsponge: return sponge;

                default:
                    if (b < 50) return b; else return 22;
            }
        }
        public static byte SaveConvert(byte b)
        {
            switch (b)
            {
                case 200:
                case 202:
                case 203:
                case 204:
                    return 0; //air_flood must be converted to air on save to prevent issues
                case 201: return 111; //door_air back into door
                case 205: return 113; //door_air back into door
                case 206: return 114; //door_air back into door
                case 207: return 115; //door_air back into door
                case 208: return 116; //door_air back into door
                case 209: return 117; //door_air back into door
                case 210: return 118; //door_air back into door
                case 211: return 119; //door_air back into door
                case 212: return 120; //door_air back into door
                case 213: return 121; //door_air back into door
                case 214: return 165; //door_air back into door
                case 215: return 166; //door_air back into door
                case 216: return 167; //door_air back into door
                case 217: return air_door; //door_air back into door

                case odoor1_air:
                case odoor2_air:
                case odoor3_air:
                case odoor4_air:
                case odoor5_air:
                case odoor6_air:
                case odoor7_air:
                case odoor8_air:
                case odoor9_air:
                case odoor10_air:
                case odoor11_air:
                case odoor12_air:
                    return Odoor(b);

                default: return b;
            }
        }
        public static byte DoorAirs(byte b)
        {
            switch (b)
            {
                case door: return door_air;
                case door2: return door2_air;
                case door3: return door3_air;
                case door4: return door4_air;
                case door5: return door5_air;
                case door6: return door6_air;
                case door7: return door7_air;
                case door8: return door8_air;
                case door9: return door9_air;
                case door10: return door10_air;
                case air_switch: return door11_air;
                case water_door: return door12_air;
                case lava_door: return door13_air;
                case air_door: return door14_air;
                default: return 0;
            }
        }

        public static bool TDoor(byte b)
        {
            switch (b)
            {
                case tdoor:
                case tdoor2:
                case tdoor3:
                case tdoor4:
                case tdoor5:
                case tdoor6:
                case tdoor7:
                case tdoor8:
                case tdoor9:
                case tdoor10:
                case tdoor11:
                case tdoor12:
                case tdoor13:
                    return true;
            }
            return false;
        }

        public static byte Odoor(byte b)
        {
            switch (b)
            {
                case odoor1: return odoor1_air;
                case odoor2: return odoor2_air;
                case odoor3: return odoor3_air;
                case odoor4: return odoor4_air;
                case odoor5: return odoor5_air;
                case odoor6: return odoor6_air;
                case odoor7: return odoor7_air;
                case odoor8: return odoor8_air;
                case odoor9: return odoor9_air;
                case odoor10: return odoor10_air;
                case odoor11: return odoor11_air;
                case odoor12: return odoor12_air;

                case odoor1_air: return odoor1;
                case odoor2_air: return odoor2;
                case odoor3_air: return odoor3;
                case odoor4_air: return odoor4;
                case odoor5_air: return odoor5;
                case odoor6_air: return odoor6;
                case odoor7_air: return odoor7;
                case odoor8_air: return odoor8;
                case odoor9_air: return odoor9;
                case odoor10_air: return odoor10;
                case odoor11_air: return odoor11;
                case odoor12_air: return odoor12;
            }
            return Zero;
        }
    }
}