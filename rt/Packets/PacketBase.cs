using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class PacketBase {
        uint _packetType;
        List<byte> _data;

        public PacketBase(uint packetType, List<byte> data = null) {

        }
    }
}
