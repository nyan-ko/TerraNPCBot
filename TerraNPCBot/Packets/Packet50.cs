using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Update player buff (50)
    /// </summary>
    public class Packet50 : PacketBase {
        /// <summary>
        /// Update player buff (50)
        /// </summary>
        public Packet50(byte plr, ushort[] buffs) : base(0x32) {
            using (Amanuensis) {
                Amanuensis.Write(plr);
                for (int i = 0; i < 22; ++i) {
                    Amanuensis.Write(buffs[i]);
                }
                Packetize();
            }
        }
    }
}
