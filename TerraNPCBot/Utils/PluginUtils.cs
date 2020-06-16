using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class PluginUtils {

        /// <summary>
        /// RegEx of all legal characters for bot names.
        /// </summary>
        // Could definitely be optimized but I have no idea how lol
        private static Regex legalCharacters = new Regex(@"[^A-Z+a-z+0-9+-+_+,+.+~+*]");
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
                // DateTime.Compare() returns a 1 if 'prune' is later than the save file creation date
                // effectively moving all files made earlier than 'prune' (0 or -1 from Compare()) to the pruned folder location
                if (DateTime.Compare(prune, fileCreation) <= 0) {
                    file.MoveTo(Path.Combine(Program.Program.PluginPrunedSaveFolderLocation, file.Name));
                }
            }
        }

        public static bool ValidateBotName(ref string name) {
            MatchCollection matchIllegals = legalCharacters.Matches(name);  // Find all characters not included in the RegEx
            if (matchIllegals.Count > 0) {
                StringBuilder mutableString = new StringBuilder(name); // Create a stringbuilder from name to be able to edit it
                foreach (Match match in matchIllegals) {
                    mutableString[match.Index] = '*';  // Censor illegal character lol
                }
                name = mutableString.ToString();
                return false;
            }
            return true;
        }
    }
}
