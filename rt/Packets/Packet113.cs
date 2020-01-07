using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Crystal invasion start (113)
    /// </summary>
    public class Packet113 : PacketBase {
        /// <summary>
        /// Crystal invasion start (113)
        /// </summary>
        public Packet113(short x, short y) : base(113, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
