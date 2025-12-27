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
using System.Threading;

namespace MCZall
{
    public class CmdRide : Command
    {
        public override string Name { get { return "ride"; } }
        public override string Shortcut { get { return ""; } }
        public override string Type { get { return "other"; } }
        public CmdRide() { }
        public override void Use(Player p, string message)
        {
            p.onTrain = !p.onTrain;

            if (p.onTrain)
            {
                p.extraThread = new Thread(new ThreadStart(delegate
                {
                start: Thread.Sleep(p.level.speedPhysics / 3);

                    ushort x = (ushort)(p.pos[0] / 32);
                    ushort y = (ushort)(p.pos[1] / 32);
                    ushort z = (ushort)(p.pos[2] / 32);

                    for (ushort xx = (ushort)(x - 1); xx <= x + 1; xx++)
                    {
                        for (ushort yy = (ushort)(y - 1); yy <= y + 1; yy++)
                        {
                            for (ushort zz = (ushort)(z - 1); zz <= z + 1; zz++)
                            {
                                if (p.level.GetTile(xx, yy, zz) == Block.train)
                                {
                                    p.invincible = true; p.trainGrab = true;
                                    byte newY = 0;

                                    if (y - yy == -1) newY = 240;
                                    else if (y - yy == 0) newY = 0;
                                    else newY = 8;

                                    unchecked
                                    {
                                        if (x - xx == -1)
                                            if (z - zz == -1) p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 96, newY);
                                            else if (z - zz == 0) p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 64, newY);
                                            else p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 32, newY);
                                        else if (x - xx == 0)
                                            if (z - zz == -1) p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 128, newY);
                                            else if (z - zz == 0) { }
                                            else p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 0, newY);
                                        else
                                            if (z - zz == -1) p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 160, newY);
                                        else if (z - zz == 0) p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 192, newY);
                                        else p.SendPos((byte)-1, (ushort)(xx * 32 + 16), (ushort)((yy + 1) * 32 - 2), (ushort)(zz * 32 + 16), 224, newY);
                                    }
                                    goto start;
                                }
                            }
                        }
                    }

                    Thread.Sleep(1000);
                    p.invincible = false;
                    p.trainGrab = false;
                    goto start;
                }));
                p.extraThread.Start();
                p.SendMessage("Stand near a train to mount it");
            }
            else
            {
                p.extraThread.Abort();
                p.trainGrab = false;
                p.extraThread = new Thread(new ThreadStart(delegate
                {
                    Thread.Sleep(1000);
                    p.invincible = false;
                    p.extraThread.Abort();
                })); p.extraThread.Start();
                p.SendMessage("Dismounted");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/ride - Rides a nearby train.");
        }
    }
}