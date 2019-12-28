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
        public Packet5(Player plr, int slot) : base(0x5, new List<byte>()) {
            AddData(plr.PlayerID.ToString());
            AddData(slot.ToString());
            AddStructuredData<ushort>(0);
            AddData(0.ToString());
            AddStructuredData<ushort>(0);
        }
    }
}
