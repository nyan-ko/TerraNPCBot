using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    public class Class1 : PacketBase {

        public Class1 (int protocol) : base(0x1, new List<byte>()){

        }
    }
}
