using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    /// <summary>
    /// Connect request (1)
    /// </summary>
    public class Packet1 : PacketBase {
        public Packet1 (int protocol) : base(0x1, new List<byte>()){
            AddData($"Terraria{protocol}", true);
        }
    }
}
