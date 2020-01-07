﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Place tile entity (87)
    /// </summary>
    public class Packet87 : PacketBase {
        /// <summary>
        /// Place tile entity (87)
        /// </summary>
        public Packet87(short x, short y, byte type) : base(87, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(type);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}