using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player hurt v2 (117)
    /// </summary>
    public class Packet117 : PacketBase {
        /// <summary>
        /// Player hurt v2 (117)
        /// </summary>
        public Packet117(byte id, Terraria.DataStructures.PlayerDeathReason reason, short dmg, byte dir, byte flags, sbyte cc) : base(117, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(id);
                reason.WriteSelfTo(Amanuensis);
                Amanuensis.Write(dmg);
                Amanuensis.Write(dir);
                Amanuensis.Write(flags);
                Amanuensis.Write(cc);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
