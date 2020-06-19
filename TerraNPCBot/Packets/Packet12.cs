using System;
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
        public Packet12(byte id, short x, short y, int respawnTimeRemaining, byte context) : base(0xC) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(respawnTimeRemaining);
                Amanuensis.Write(context);
                Packetize();
            }
        }
    }
}
