﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player mana (42)
    /// </summary>
    public class Packet42 : PacketBase {
        /// <summary>
        /// Player mana (42)
        /// </summary>
        public Packet42(byte plr, short mana, short max) : base(0x2a, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr);
                Amanuensis.Write(mana);
                Amanuensis.Write(max);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
