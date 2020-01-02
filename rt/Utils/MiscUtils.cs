using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace rt.Utils {
    public static class MiscUtils {
        public static void WriteColor(this System.IO.BinaryWriter writer, Microsoft.Xna.Framework.Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static void MultiMsg(this TSPlayer player, List<string> msgs, Microsoft.Xna.Framework.Color color) {
            foreach (var x in msgs) {
                player.SendMessage(x, color);
            }
            player.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static void GenericMsg(this TSPlayer pl) {

        }
    }
}
