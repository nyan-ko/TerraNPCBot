﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Continue connecting (3)
    /// </summary>
    public class Packet3 : ParsedPacketBase {
        public Packet3(Bot plr, byte id) : base(0x3) {
            plr.ID = id;
        }
    }
}