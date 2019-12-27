using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    class Packet0x2 : PacketBase {
        public Packet0x2 (string reason) : base(0x2, new List<byte>()) {

        }
    }
}
