using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace TerraNPCBot {
    /// <summary>
    /// Bot Terraria Server player, not a seven member South Korean boy band player.
    /// </summary>
    public class BTSPlayer {
        public static BTSPlayer BTSServerPlayer = new BTSPlayer();

        internal BTSPlayer() {
            ownedBots = new List<Bot>();
            selected = -1;
        }

        public BTSPlayer(int index) {
            ownedBots = new List<Bot>();
            selected = -1;

            ServerIndex = index;
        }

        public int ServerIndex { get; private set; }
        private int selectedDelete;
        public int SelectedDelete {
            get {
                int del = selectedDelete;
                selectedDelete = -1;
                return del;
            }
            set { selectedDelete = value; }
        }

        public uint botLimit = 10;
        public List<Bot> ownedBots;
        public int selected;

        public bool autosave = true;
        public bool canBeTeleportedTo = false;
        public bool canBeCopied = false;

        /// <summary>
        /// Finds a bot given its name or index within a player's owned bots.
        /// </summary>
        /// <param name="nameOrIndex"></param>
        /// <returns></returns>
        public List<Bot> GetBotFromIndexOrName(string nameOrIndex) {
            List<Bot> found = new List<Bot>();
            nameOrIndex = nameOrIndex.ToLower();

            // Index search
            if (int.TryParse(nameOrIndex, out int index)) {
                if (index < ownedBots.Count && index >= 0 && ownedBots?[index] != null)
                    return new List<Bot> { ownedBots[index] };
                else {
                    return found;
                }
            }

            // Name search
            foreach (Bot bot in ownedBots) {
                if (bot.Name.ToLower() == nameOrIndex)
                    return new List<Bot> { bot };
                if (bot.Name.ToLower().StartsWith(nameOrIndex))
                    found.Add(bot);
            }

            return found;
        }

        public Bot SelectedBot {
            get { return ownedBots.Count > 0 && selected != -1 ? ownedBots[selected] : null; }
        }

        public TSPlayer SPlayer {
            get { return TShock.Players[ServerIndex]; }
        }
    }
}
