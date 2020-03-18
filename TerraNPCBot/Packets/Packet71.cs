using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Packets {
    /// <summary>
    /// Release npc (71)
    /// </summary>
    public class Packet71 : PacketBase {
        /// <summary>
        /// Release npc (71)
        /// </summary>
        public Packet71(int x, int y, short type, byte style) : base(71) {
            using (Amanuensis) {
                Amanuensis.Write(x);
                Amanuensis.Write(y);
                Amanuensis.Write(type);
                Amanuensis.Write(style);
                AddData(Amanuensis.BaseStream);
            }
        }
    }
}
