using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Place item frame (89)
    /// </summary>
    public class Packet89 : PacketBase {
        /// <summary>
        /// Place item frame (89)
        /// </summary>
        public Packet89(short x, short y, short id, byte pre, short stack) : base(89, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(id);
                Amanuensis.Write(pre);
                Amanuensis.Write(stack);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
