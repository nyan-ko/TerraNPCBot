using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Player team (45)
    /// </summary>
    public class Packet45 : PacketBase {
        public Packet45(Player plr, byte team) : base(45, new List<byte>()) {
            using (Amanuensis = new System.IO.BinaryWriter(new System.IO.MemoryStream())) {
                Amanuensis.Write(plr.PlayerID);
                Amanuensis.Write(team);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
