using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Add player buff (55)
    /// </summary>
    public class Packet55 : PacketBase {
        /// <summary>
        /// Add player buff (55)
        /// </summary>
        public Packet55(byte id, byte buff, int time) : base(55, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write(buff);
                Amanuensis.Write(time);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
