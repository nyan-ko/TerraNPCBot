using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using TerraNPCBot.Data;
using TerraNPCBot.Program;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace TerraNPCBot.Utils {
    public static class StringUtils {

        /// <summary>
        /// RegEx of all legal characters for bot names.
        /// </summary>
        // Could definitely be optimized but I have no idea how lol
        private static Regex legalCharacters = new Regex(@"[^A-Z+a-z+0-9+-+_+,+.+~+*]");

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

        public static string CreateBotName(IEnumerable<string> nameRange, TSPlayer player) {
            string name = "Michael Jackson";

            string tempName = JoinAndTrimList(nameRange);
            if (tempName.Length > 30) {
                player?.SendInfoMessage("Specified name exceeds 30 character limit. Defaulting to Michael Jackson.");
                tempName = "Michael Jackson";
            }
            else if (tempName.Length < 1) {
                player?.SendInfoMessage("Specified name had a shorter length than expected. Defaulting to Michael Jackson.");
                tempName = "Michael Jackson";
            }
            else if (!ValidateBotName(ref tempName)) {
                player?.SendInfoMessage("Found and replaced illegal characters in name.");
            }
            name = tempName;
            return name;
        }

        public static string JoinAndTrimList(IEnumerable<string> parameters) {
            return string.Join(" ", parameters).Trim('"');
        }
    }
}
