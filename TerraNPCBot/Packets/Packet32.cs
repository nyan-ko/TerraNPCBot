using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Update chest item (32)
    /// </summary>
    public class Packet32 : PacketBase {
        /// <summary>
        /// Update chest item (32)
        /// </summary>
        public Packet32(short id, byte slot, short stack, byte prefix, short netid) : base(32) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(slot);
                Amanuensis.Write(stack);
                Amanuensis.Write(prefix);
                Amanuensis.Write(netid);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
