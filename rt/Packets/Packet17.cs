using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Modify tile (17)
    /// </summary>
    public class Packet17 : PacketBase {
        /// <summary>
        /// Modify tile (17)
        /// </summary>
        public Packet17(byte action, short tilex, short tiley, short var1, byte var2) : base(0x11, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(action);
                Amanuensis.Write(tilex);
                Amanuensis.Write(tiley);
                Amanuensis.Write(var1);
                Amanuensis.Write(var2);
                AddData(Amanuensis.BaseStream);
            }
        }
    }

    //Flag102
    /// <summary>
    /// Modify tile (17) server ->
    /// </summary>
    public class Packet17Parser : PacketBase {
        public Packet17Parser(byte action, short tilex, short tiley, short var1, byte var2) : base(0x11, new List<byte>()) {

        }
    }
}
