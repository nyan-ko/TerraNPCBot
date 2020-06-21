using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    [Obsolete]
    /// <summary>
    /// Crystal invasion start (113)
    /// </summary>
    public class Packet113 : PacketBase {
        /// <summary>
        /// Crystal invasion start (113)
        /// </summary>
        public Packet113(short x, short y) : base(113) {
            using (Amanuensis) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Packetize();
            }
        }
    }
}
