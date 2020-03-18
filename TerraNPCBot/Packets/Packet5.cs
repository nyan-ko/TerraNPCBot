using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player inventory slot (5)
    /// </summary>
    public class Packet5 : PacketBase {
        /// <summary>
        /// Player inventory slot (5)
        /// </summary>
        public Packet5(byte plr, byte slot, short stack = 0, byte prefix = 0, short id = 0) : base(0x5) {
            using (Amanuensis) {
                Amanuensis.Write(plr);
                Amanuensis.Write(slot);
                Amanuensis.Write(stack);
                Amanuensis.Write(prefix);
                Amanuensis.Write(id);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
