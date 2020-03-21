using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Get section (8)
    /// </summary>
    public class Packet8 : PacketBase {
        /// <summary>
        /// Get section (8)
        /// </summary>
        public Packet8() : base(0x8) {
            using (Amanuensis) {
                Amanuensis.Write(-1);
                Amanuensis.Write(-1);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
