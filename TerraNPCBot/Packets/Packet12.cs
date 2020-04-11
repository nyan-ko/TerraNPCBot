﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Spawn player (12)
    /// </summary>
    public class Packet12 : PacketBase {
        /// <summary>
        /// Spawn player (12)
        /// </summary>
        public Packet12(byte id, short x, short y) : base(0xC) { 
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Packetize();
            }
        }

        public Packet12(byte id, Microsoft.Xna.Framework.Vector2 pos) : base(12) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write((short)pos.X);
                Amanuensis.Write((short)pos.Y);
                Packetize();
            }
        }
    }
}
