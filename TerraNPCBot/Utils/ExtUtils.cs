using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

namespace TerraNPCBot.Utils {
    public static class ExtUtils {
        public static void Write(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }
    }
}
