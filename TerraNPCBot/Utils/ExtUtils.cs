using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.Server;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class ExtUtils {
        public static void Write(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static void WriteColor(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static Color ReadColor(this BinaryReader reader) {
            return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public static void MultiMsg(this TSPlayer player, List<string> msgs, Microsoft.Xna.Framework.Color color) {
            foreach (var x in msgs) {
                player?.SendMessage(x, color);
            }
            player?.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static void MultiMsg(this TSPlayer player, Color color, params string[] msgs) {
            foreach (var x in msgs) {
                player?.SendMessage(x, color);
            }
            player?.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }
    }
}
