﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player mana (42)
    /// </summary>
    public class Packet42 : PacketBase {
        /// <summary>
        /// Player mana (42)
        /// </summary>
        public Packet42(byte plr, short mana, short max) : base(0x2a) {
            using (Amanuensis) {
                Amanuensis.Write(plr);
                Amanuensis.Write(mana);
                Amanuensis.Write(max);
                Packetize();
            }
        }
    }
}
