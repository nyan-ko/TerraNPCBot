using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Connect request (1)
    /// </summary>
    public class Packet1 : PacketBase {
        /// <summary>
        /// Connect request (1)
        /// </summary>
        public Packet1 (int protocol) : base(0x1){
            using (Amanuensis) {
                Amanuensis.Write($"Terraria{protocol}");
                Packetize();
            }
        }
    }
}
