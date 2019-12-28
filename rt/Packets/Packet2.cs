﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Disconnect (2)
    /// </summary>
    class Packet2 : PacketBase {
        string reason;
        public Packet2 (string str1) : base(0x2, new List<byte>()) {
            reason = str1;
        }
    }
}