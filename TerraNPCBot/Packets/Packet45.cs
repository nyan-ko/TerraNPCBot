using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Player team (45)
    /// </summary>
    public class Packet45 : PacketBase {
        /// <summary>
        /// Player team (45)
        /// </summary>
        public Packet45(byte plr, byte team) : base(45, new List<byte>()) {
            using (Amanuensis) {
                Amanuensis.Write(plr);
                Amanuensis.Write(team);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
