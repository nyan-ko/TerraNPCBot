using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace rt {
    public class World {
        public int Time;
        public byte DayInfo;
        public byte MoonPhase;
        public short SpawnX;
        public short SpawnY;

        public int[,] Tiles;

        public void InitalizeTiles(short height, short girth) {
            Tiles = new int[height, girth];
        }
    }
}
