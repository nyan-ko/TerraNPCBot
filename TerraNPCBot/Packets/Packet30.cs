﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Update pvp (30)
    /// </summary>
    public class Packet30 : PacketBase {
        /// <summary>
        /// Update pvp (30)
        /// </summary>
        public Packet30(byte plr, bool s) : base(30) {
            using (Amanuensis) {
                Amanuensis.Write(plr);
                Amanuensis.Write(s);
                Packetize();
            }
        }
    }
}
