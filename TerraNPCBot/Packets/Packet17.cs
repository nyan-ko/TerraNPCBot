using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Modify tile (17)
    /// </summary>
    public class Packet17 : PacketBase {
        /// <summary>
        /// Modify tile (17)
        /// </summary>
        public Packet17(byte action, short tilex, short tiley, short var1, byte var2) : base(0x11) {
            using (Amanuensis) {
                Amanuensis.Write(action);
                Amanuensis.Write(tilex);
                Amanuensis.Write(tiley);
                Amanuensis.Write(var1);
                Amanuensis.Write(var2);
                AddData(Amanuensis.BaseStream);
            }
        }
    }

    /// <summary>
    /// Modify tile (17) server ->
    /// </summary>
    public class Packet17Parser : ParsedPacketBase {
        public Packet17Parser(System.IO.Stream str) : base(17) {
            using (_data = new System.IO.MemoryStream()) {
                str.CopyTo(_data);
            }
        }
    }
}
