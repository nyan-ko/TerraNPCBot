using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Forcefully sent player active packet for players ignoring bots
    /// </summary>
    public class IgnorePacket : PacketBase {
        /// <summary>
        /// Forcefully sent player active packet for players ignoring bots
        /// </summary>
        public IgnorePacket(byte id, bool active) : base(14) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(active);

                // Write normal player active packet to byte array with packet type 14
                Packetize();
                // Set packet type to exclusive plugin packet type so it is recognized by bots
                // without changing actual data of the packet
                packetType = 254;
            }
        }
    }
}
