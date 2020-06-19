using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Update player (13)
    /// </summary>
    public class Packet13 : PacketBase {

        /// <summary>
        /// Update player (13)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="control"></param>
        /// <param name="pulley"></param>
        /// <param name="misc"></param>
        /// <param name="sleepingInfo"></param>
        /// <param name="selected"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="vecX"></param>
        /// <param name="vecY"></param>
        /// <param name="origPosX"></param>
        /// <param name="origPosY"></param>
        /// <param name="homePosX"></param>
        /// <param name="homePosY"></param>
        // method parameters be like
        public Packet13(byte id, byte control, byte pulley, byte misc, byte sleepingInfo, byte selected,
            float posX, float posY, float vecX = 0, float vecY = 0, float origPosX = 0, float origPosY = 0, float homePosX = 0, float homePosY = 0) : base(0xD) {
            using (Amanuensis = new BinaryWriter(new MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(control);
                Amanuensis.Write(pulley);
                Amanuensis.Write(misc);
                Amanuensis.Write(sleepingInfo);
                Amanuensis.Write(selected);
                Amanuensis.Write(posX);
                Amanuensis.Write(posY);
                if ((pulley & 4) == 4) {   // UpdateVelocity flag
                    Amanuensis.Write(vecX);
                    Amanuensis.Write(vecY);
                }
                if ((misc & 64) == 64) {   // UsedPotionOfReturn flag
                    Amanuensis.Write(origPosX);
                    Amanuensis.Write(origPosY);
                    Amanuensis.Write(homePosX);
                    Amanuensis.Write(homePosY);
                }
                Packetize();
            }
            Program.Program.Bots[id].Position = new Vector2(posX, posY);
        }
    }
}
