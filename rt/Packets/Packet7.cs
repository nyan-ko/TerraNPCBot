using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Packets {
    public class Packet7 : PacketBase {
        public Packet7 (World wrld, Player plr, int time, byte dayinfo, byte moon,
            short maxX, short maxY, short spawnX, short spawnY) : base (0x7, new List<byte>()) {
            wrld.DayInfo = dayinfo;
            wrld.MoonPhase = moon;
            wrld.Time = time;
            wrld.SpawnX = spawnX;
            wrld.SpawnY = spawnY;

            if (!plr.Initialized) {
                wrld.InitalizeTiles(maxX, maxY);
            }
        }
    }
}
