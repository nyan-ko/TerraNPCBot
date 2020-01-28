using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Open chest (31)
    /// </summary>
    public class Packet31 : PacketBase {
        /// <summary>
        /// Open chest (31)
        /// </summary>
        public Packet31(short x, short y) : base(31, new List<byte>()){
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
