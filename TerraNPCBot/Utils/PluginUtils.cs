using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace TerraNPCBot.Utils {
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
                player?.SendMessage(x, color);
            }
            player?.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
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
            string oldpath = path.Insert(path.Length - 4, $"~{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}~");
            if (File.Exists(oldpath)) {
                File.Delete(oldpath);
            }
            File.Copy(path, oldpath);
        }

        public static void PruneSaves(DateTime prune) {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Program.Program.PluginSaveFolderLocation);
            foreach (var file in dir.GetFiles()) {
                DateTime fileCreation = file.CreationTime;
                if (DateTime.Compare(prune, fileCreation) <= 0) {
                    file.MoveTo(Path.Combine(Program.Program.PluginPrunedSaveFolderLocation, file.Name));
                }
            }
        }
    }
}
