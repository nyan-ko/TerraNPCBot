using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Play music item (58)
    /// </summary>
    public class Packet58 : PacketBase {
        /// <summary>
        /// Play music item (58)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        public Packet58(byte id, float note) : base(58) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(note);
                Packetize();
            }
        }
    }
}
