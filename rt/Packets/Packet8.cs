using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Get section (8)
    /// </summary>
    public class Packet8 : PacketBase {
        public Packet8() : base(0x8, new List<byte>()) {
            AddStructuredData<int>(-1);
            AddStructuredData<int>(-1);
        }
    }
}
