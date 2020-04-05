using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {

    public class Packet58 : PacketBase {

        public Packet58(byte id, float note) : base(58) {
            using (Amanuensis) {
                Amanuensis.Write(id);
                Amanuensis.Write(note);
                Packetize();
            }
        }
    }
}
