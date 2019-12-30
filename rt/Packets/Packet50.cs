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
        public Packet50(Player plr) : base(0x32, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                for (int i = 0; i < 22; i++) {
                    Amanuensis.Write((byte)0);
                }
            }
            AddData(Amanuensis.BaseStream);
        }
    }
}
