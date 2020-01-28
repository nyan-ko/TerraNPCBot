using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Placeholder (67), unused for now
    /// </summary>
    public class Packet67 : PacketBase {
        /// <summary>
        /// Placeholder (67), unused for now
        /// </summary>
        public Packet67() : base(67, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {

            }
        }
    }
}
