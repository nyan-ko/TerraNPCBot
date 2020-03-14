using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Spawn boss invasion (61)
    /// </summary>
    public class Packet61 : PacketBase {
        /// <summary>
        /// Spawn boss invasion (61)
        /// </summary>
        public Packet61(short id, short type) : base(61, new List<byte>()) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(type);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
