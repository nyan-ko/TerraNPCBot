using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player hp (16)
    /// </summary>
    public class Packet16 : PacketBase {
        public Packet16(Player plr) : base(0x10, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
                Amanuensis.Write(plr.CurHP);
                Amanuensis.Write(plr.MaxHP);
            }
            AddData(Amanuensis.BaseStream);
        }
    }
}
