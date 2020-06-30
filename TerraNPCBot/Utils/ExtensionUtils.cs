using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.Server;
using Terraria;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class ExtensionUtils {
        public static void SendMoreThanOneMatchError(this TSPlayer tplayer, string searchTerm, IEnumerable<object> matches) {
            tplayer.SendErrorMessage($"Multiple items matched your search term: \"{searchTerm}\".");

            PaginationTools.BuildLinesFromTerms(matches).ForEach(tplayer.SendErrorMessage);

            tplayer.SendErrorMessage($"Use a more specific search term or directly refer to the item's index, if possible.");
        }

        public static void SendNoMatchError(this TSPlayer tplayer, string searchTerm) {
            tplayer.SendErrorMessage($"No items matched your search term: \"{searchTerm}\".");
        }

        public static void WriteColor(this BinaryWriter writer, Color color) {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static Color ReadColor(this BinaryReader reader) {
            return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public static void MultiMsg(this TSPlayer player, Color color, params string[] msgs) {
            foreach (var x in msgs) {
                player?.SendMessage(x, color);
            }
            player?.SendMessage("Use ↑/↓ to scroll through the message.", Microsoft.Xna.Framework.Color.Yellow);
        }

        public static void MultiMsg(this TSPlayer player, string message, Color color) {
            MultiMsg(player, color, message.Split('\\'));
        }

        public static bool HandleListFromSearches(this TSPlayer tp, string searchTerm, IEnumerable<object> list) {
            int count = list.Count();
            if (count == 1)
                return true;
            else if (count == 0) {
                tp.SendNoMatchError(searchTerm);
                return false;
            }
            else {
                tp.SendMoreThanOneMatchError(searchTerm, list);
                return false;
            }
        }

        public static BTSPlayer ToBTSPlayer(this TSPlayer t) {
            return Program.PluginMain.Players[t.Index];
        }
    }
}
