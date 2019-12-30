using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player inventory slot (5)
    /// </summary>
    public class Packet5 : PacketBase {
        public Packet5(Player plr, byte slot) : base(0x5, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
                Amanuensis.Write(slot);
                Amanuensis.Write((ushort)0);
                Amanuensis.Write((byte)0);
                Amanuensis.Write((ushort)0);
            }
            AddData(Amanuensis.BaseStream);
        }
    }
}
