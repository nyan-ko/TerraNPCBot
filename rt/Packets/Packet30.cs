using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Update pvp (30)
    /// </summary>
    public class Packet30 : PacketBase {
        public Packet30(Player plr, bool s) : base(30, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
                Amanuensis.Write(s);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
