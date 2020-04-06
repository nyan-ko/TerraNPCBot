using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.Localization;
using TerraNPCBot.Utils;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Load net module (82)
    /// </summary>
    public class Packet82 : PacketBase {
        /// <summary>
        /// Load net module (82) - only used for text :sad:
        /// </summary>
        public Packet82(string message, byte id, Microsoft.Xna.Framework.Color color) : base(82) {
            using (Amanuensis) {
                Amanuensis.Write((ushort)1);
                Amanuensis.Write(id);
                new NetworkText(message, NetworkText.Mode.Literal).Serialize(Amanuensis);
                Amanuensis.Write(color);
                Packetize();
            }
        }
    }
}
