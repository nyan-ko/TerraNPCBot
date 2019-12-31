using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    public class Packet7 : ParsedPacketBase {
        public Packet7 (BinaryReader r, World wrld, Player plr) : base (0x7) {
            using (r) {
                wrld.Time = r.ReadInt32();
                wrld.DayInfo = r.ReadByte();
                wrld.MoonPhase = r.ReadByte();
                short maxX = r.ReadInt16();
                short maxY = r.ReadInt16();
                wrld.SpawnX = r.ReadInt16();
                wrld.SpawnY = r.ReadInt16();

                if (!plr.Initialized) {
                    wrld.InitalizeTiles(maxX, maxY);
                }
            }
        }
    }
}
