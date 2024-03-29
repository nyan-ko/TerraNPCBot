﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Gem lock toggle (105)
    /// </summary>
    public class Packet105 : PacketBase {
        /// <summary>
        /// Gem lock toggle (105)
        /// </summary>
        public Packet105(short x, short y, bool on) : base(105){
            using (Amanuensis) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(on);
                Packetize();
            }
        }
    }
}
