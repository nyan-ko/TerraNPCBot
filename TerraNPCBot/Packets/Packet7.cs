using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// World info (7)
    /// </summary>
    public class Packet7 : ParsedPacketBase {
        /// <summary>
        /// World info (7)
        /// </summary>
        public Packet7 (BinaryReader r, Player plr) : base (0x7) {
            
        }
    }
}
