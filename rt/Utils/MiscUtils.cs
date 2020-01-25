using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.IO.Compression;
using Terraria;
using TShockAPI;

namespace rt.Utils {
    public static class PluginUtils {
        public static void WriteColor(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static Color ReadColor(this BinaryReader reader) {
            return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public static void WriteItem(this BinaryWriter writer, Item item, byte i) {
            writer.Write(i);
            writer.Write((short)item.stack);
            writer.Write(item.prefix);
            writer.Write((short)item.netID);
        }

        public static void MultiMsg(this TSPlayer player, List<string> msgs, Microsoft.Xna.Framework.Color color) {
            foreach (var x in msgs) {
                player.SendMessage(x, color);
            }
            player.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static string GetSavePath(int id) {
            string path = Path.Combine(Program.Program.PluginFolderLocation, $"bplayer-{id}.dat");
            CopyOld(path);
            return path;
        }

        public static void CopyOld(string path) {
            if (!File.Exists(path))
                return;
            var now = DateTime.Now;
            string oldpath = path.Insert(path.Length - 5, $"|{now.Year}_{now.Month}_{now.Day}_{now.Hour}|");
            File.Copy(path, oldpath);
        }
    }
}
