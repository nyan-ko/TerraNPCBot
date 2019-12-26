using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    public class Packet0x1 : PacketBase {

        public Packet0x1 (int protocol) : base(0x1, new List<byte>()){

        }
    }
}
