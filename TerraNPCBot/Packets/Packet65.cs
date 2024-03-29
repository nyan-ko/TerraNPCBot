﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player npc teleport (65)
    /// </summary>
    public class Packet65 : PacketBase {
        /// <summary>
        /// Player npc teleport (65)
        /// </summary>
        public Packet65(byte flag, short target, float x, float y) : base(65) {
            using (Amanuensis) {
                Amanuensis.Write(flag);
                Amanuensis.Write(target);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Packetize();
            }
        }
    }
}
