using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    public class Packet6 : PacketBase {
        public Packet6() : base(0x6, new List<byte>()) {

        }
    }
}
