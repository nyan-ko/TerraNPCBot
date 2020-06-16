using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Tells the bot to stop its write thread when read from its write queue.
    /// </summary>
    public class ShutdownPacket : PacketBase {
        /// <summary>
        /// Tells the bot to stop its write thread when read from its write queue.
        /// </summary>
        public ShutdownPacket() : base(byte.MaxValue) {

        }
    }
}
