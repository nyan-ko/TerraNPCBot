using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Sync tile picking or whatever
    /// </summary>
    public class Packet125 : PacketBase {
        public Packet125(byte id, short x, short y, byte damage) : base(125) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(damage);

                Packetize();
            }
        }
    }
}
