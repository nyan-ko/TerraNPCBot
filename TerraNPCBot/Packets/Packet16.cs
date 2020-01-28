using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player hp (16)
    /// </summary>
    public class Packet16 : PacketBase {
        /// <summary>
        /// Player hp (16)
        /// </summary>
        public Packet16(byte plr, short hp, short max) : base(0x10, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr);
                Amanuensis.Write(hp);
                Amanuensis.Write(max);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
