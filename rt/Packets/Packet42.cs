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
            AddData(plr.PlayerID.ToString());
            AddStructuredData<ushort>(plr.CurMana);
            AddStructuredData<ushort>(plr.MaxMana);
        }
    }
}
