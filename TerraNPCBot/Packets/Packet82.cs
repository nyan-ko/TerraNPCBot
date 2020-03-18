using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Load net module (82)
    /// </summary>
    public class Packet82 : PacketBase {
        /// <summary>
        /// Load net module (82) - only used for text :sad:
        /// </summary>
        public Packet82(string message) : base(82) {
            using (Amanuensis) {
                Amanuensis.Write((ushort)1);
                new Terraria.Chat.ChatMessage(message).Serialize(Amanuensis);

                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
