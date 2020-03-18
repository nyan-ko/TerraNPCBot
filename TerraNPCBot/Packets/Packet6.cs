using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Request world data (6)
    /// </summary>
    [Obsolete]
    public class Packet6 : PacketBase {
        /// <summary>
        /// Request world data (6)
        /// </summary>
        [Obsolete]
        public Packet6() : base(0x6, new List<byte>()) {

        }
    }
}
