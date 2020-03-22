using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Send UUID (68)
    /// </summary>
    public class Packet68 : PacketBase {
        public Packet68() : base(68) {
            using (Amanuensis) {
                Amanuensis.Write(Client.UUID);
                Packetize();
            }
        }
    }
}
