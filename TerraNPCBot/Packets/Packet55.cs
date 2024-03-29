﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Add player buff (55)
    /// </summary>
    public class Packet55 : PacketBase {
        /// <summary>
        /// Add player buff (55)
        /// </summary>
        public Packet55(byte id, byte buff, int time) : base(55) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(buff);
                Amanuensis.Write(time);
                Packetize();
            }
        }
    }
}
