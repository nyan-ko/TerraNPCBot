using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player mana (42)
    /// </summary>
    public class Packet42 : PacketBase {
        public Packet42(Player plr) : base(0x2a, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
                Amanuensis.Write(plr.CurMana);
                Amanuensis.Write(plr.MaxMana);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
