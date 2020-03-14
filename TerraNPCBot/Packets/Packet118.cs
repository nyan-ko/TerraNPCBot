using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player death v2 (118)
    /// </summary>
    public class Packet118 : PacketBase {
        /// <summary>
        /// Player death v2 (118)
        /// </summary>
        public Packet118(byte id, Terraria.DataStructures.PlayerDeathReason reason, short dmg, byte dir, byte flags) : base(118, new List<byte>()) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                reason.WriteSelfTo(Amanuensis);
                Amanuensis.Write(dmg);
                Amanuensis.Write(dir);
                Amanuensis.Write(flags);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
