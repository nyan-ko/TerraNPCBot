using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Mass wire operation (109)
    /// </summary>
    public class Packet109 : PacketBase {
        /// <summary>
        /// Mass wire operation (109)
        /// </summary>
        public Packet109(short sx, short sy, short ex, short ey, byte mode) : base(109, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(sx);
                Amanuensis.Write(sy);
                Amanuensis.Write(ex);
                Amanuensis.Write(ey);
                Amanuensis.Write(mode);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
