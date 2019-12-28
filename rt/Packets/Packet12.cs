using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace rt.Packets {
    /// <summary>
    /// Spawn player (12)
    /// </summary>
    public class Packet12 : PacketBase {
        public Packet12(int id) : base(0xC, new List<byte>()) {
            AddData(id.ToString());
            AddStructuredData<short>(Main.spawnTileX);
            AddStructuredData<short>(Main.spawnTileY);
        }
    }
}
