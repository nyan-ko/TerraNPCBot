﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player item animation (41)
    /// </summary>
    public class Packet41 : PacketBase {
        /// <summary>
        /// Player item animation (41)
        /// </summary>
        public Packet41(byte id, float rot, short ani) : base(41, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(rot);
                Amanuensis.Write(ani);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
