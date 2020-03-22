using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player active (14)
    /// </summary>
    public class Packet14 : PacketBase {
        /// <summary>
        /// Player active (14)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="active"></param>
        public Packet14(byte id, bool active) : base(14) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(active);
                Packetize();
            }
        }
    }
}
