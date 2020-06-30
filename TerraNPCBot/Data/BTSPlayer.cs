using System.Collections.Generic;
using System.IO;
using Terraria;
using TShockAPI;

namespace TerraNPCBot {
    /// <summary>
    /// Bot Terraria Server player, not a seven member South Korean boy band player.
    /// </summary>
    public class BTSPlayer {
        public static BTSPlayer BTSServerPlayer = new BTSPlayer();

        internal BTSPlayer(DatabaseItems.DBPlayer dbPlayer, int index) {
            BotLimit = dbPlayer.BotLimit;
            CanBeTeleportedTo = dbPlayer.Teleportable;
            CanBeCopied = dbPlayer.Copyable;
            IgnoreBots = dbPlayer.Ignoring;

            List<Bot> bots = new List<Bot>();
            int indexInOwnedBots = 0;
            foreach (var bot in dbPlayer.OwnedBots) {
                bots.Add(bot.ConvertDBItem(index, indexInOwnedBots));
                indexInOwnedBots++;
            }

            OwnedBots = bots;
            GroupedBots = new List<int>();
            Index = index;
        }

        internal BTSPlayer() {
            OwnedBots = new List<Bot>();
            GroupedBots = new List<int>();

            Index = -1;
        }

        public BTSPlayer(int index) {
            OwnedBots = new List<Bot>();
            GroupedBots = new List<int>();

            Index = index;
        }

        public int Index { get; private set; }

        private int selectedDelete;

        public int GetSelectedDelete() {
            int del = selectedDelete;
            selectedDelete = -1;
            return del;
        }

        public void SetSelectedDelete(int selectedIndex) { selectedDelete = selectedIndex; }

        public uint BotLimit { get; set; } = 5; // Arbitrary limit
        public List<Bot> OwnedBots { get; set; }
        public List<int> GroupedBots { get; set; }
        public int Selected { get; set; } = -1;

        public bool CanBeTeleportedTo { get; set; } = false;
        public bool CanBeCopied { get; set; } = false;
        public bool IgnoreBots { get; private set; } = false;

        public void ToggleBotVisibility(bool visible) {
            if (visible) {
                foreach (Bot bot in Program.PluginMain.GlobalRunningBots) {
                    // Resend join packets to the player
                    bot.RequestJoinPackets(Index);
                }
            }
            else {
                foreach (Bot bot in Program.PluginMain.GlobalRunningBots) {
                    // IgnorePacket bypasses player ignores to tell them bot has disconnected
                    bot.Client.QueuePackets(new Packets.IgnorePacket(bot.ID, false).AddTargets(Index));
                }
            }

            IgnoreBots = !visible;
        }

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
                index--;  // Indices specified through commands are not zero-based because front-end shenanigans
                if (index < OwnedBots.Count && index >= 0 && OwnedBots?[index] != null)
                    return new List<Bot> { OwnedBots[index] };
                else {
                    return found;
                }
            }

            // Name search
            foreach (Bot bot in OwnedBots) {
                if (bot.Name.ToLower() == nameOrIndex)
                    return new List<Bot> { bot };
                if (bot.Name.ToLower().StartsWith(nameOrIndex))
                    found.Add(bot);
            }

            return found;
        }

        public Bot SelectedBot {
            get { return OwnedBots.Count > 0 && Selected != -1 ? OwnedBots[Selected] : null; }
        }

        public TSPlayer SPlayer {
            get { return TShock.Players[Index]; }
        }
    }
}
