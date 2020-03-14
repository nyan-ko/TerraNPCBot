using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Door toggle (19)
    /// </summary>
    public class Packet19 : PacketBase {
        /// <summary>
        /// Door toggle (19)
        /// </summary>
        public Packet19(byte action, short x, short y, byte direction) : base(19, new List<byte>()) {
            using (Amanuensis) {
                Amanuensis.Write(action);
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(direction);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
