using System;
using System.IO;
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
        public Packet12(byte id) : base(0xC, new List<byte>()) { 
            using (Amanuensis = new BinaryWriter(new MemoryStream())) {
                Amanuensis.Write(id);
                Amanuensis.Write((short)Main.spawnTileX);
                Amanuensis.Write((short)Main.spawnTileY);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
