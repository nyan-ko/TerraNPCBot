using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Kill portal (95)
    /// </summary>
    public class Packet95 : PacketBase {
        /// <summary>
        /// Kill portal (95)
        /// </summary>
        public Packet95(ushort index) : base(95) {
            using (Amanuensis) {
                Amanuensis.Write(index);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
