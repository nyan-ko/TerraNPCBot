using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Kill portal (95)
    /// </summary>
    public class Packet95 : PacketBase {
        /// <summary>
        /// Kill portal (95)
        /// </summary>
        public Packet95(ushort index) : base(95, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(index);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
