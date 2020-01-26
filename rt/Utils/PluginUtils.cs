using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
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

        public static void MultiMsg(this TSPlayer player, List<string> msgs, Microsoft.Xna.Framework.Color color) {
            foreach (var x in msgs) {
                player.SendMessage(x, color);
            }
            player.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static string AllocateSavePath(int id) {
            string path = Path.Combine(Program.Program.PluginSaveFolderLocation, $"bplayer-{id}.dat");
            CreateOld(path);
            return path;
        }

        public static void CreateOld(string path) {
            if (!File.Exists(path))
                return;
            var now = DateTime.Now;
            string oldpath = path.Insert(path.Length - 4, $"~{now.Year}_{now.Month}_{now.Day}_{now.Hour}~");
            File.Copy(path, oldpath);
        }

        public static void PruneSaves(DateTime prune) {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Program.Program.PluginSaveFolderLocation);
            foreach (var file in dir.GetFiles()) {
                string date = file.Name.Substring(file.Name.IndexOf('~'), file.Name.Length - file.Name.LastIndexOf('~'));
                var split = date.Split('_');
                if (!int.TryParse(split[0], out int years) ||
                    !int.TryParse(split[1], out int months) ||
                    !int.TryParse(split[2], out int days) ||
                    !int.TryParse(split[3], out int hours)) {
                    continue;
                }
                DateTime fileCreation = new DateTime(years, months, days, hours, 0, 0);
                if (DateTime.Compare(prune, fileCreation) <= 0) {
                    file.MoveTo(Program.Program.PluginPrunedSaveFolderLocation);
                }
            }
        }
    }
}
