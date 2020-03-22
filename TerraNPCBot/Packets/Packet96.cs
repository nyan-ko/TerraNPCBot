using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player teleport portal (96)
    /// </summary>
    public class Packet96 : PacketBase {
        /// <summary>
        /// Player teleport portal (96)
        /// </summary>
        public Packet96(byte id, short portal, float x, float y, float vecx, float vecy) : base(87) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(portal);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(vecx);
                Amanuensis.Write(vecy);
                Packetize();
            }
        }
    }
}
