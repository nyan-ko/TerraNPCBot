using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    public class RawPacket : PacketBase {
        public RawPacket(byte type, byte[] data) : base(type) {
            using (Amanuensis) {
                Amanuensis.Write(data);

                Packetize();
            }
        }
    }
}
