using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Continue connecting 2 (6)
    /// </summary>
    public class Packet6 : PacketBase {
        public Packet6() : base(0x6, new List<byte>()) {

        }
    }
}
