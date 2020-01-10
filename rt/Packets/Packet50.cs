using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Update player buff (50)
    /// </summary>
    public class Packet50 : PacketBase {
        /// <summary>
        /// Update player buff (50)
        /// </summary>
        public Packet50(byte plr, byte[] buffs) : base(0x32, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr);
                for (int i = 0; i < 22; ++i) {
                    Amanuensis.Write(buffs[i]);
                }
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
