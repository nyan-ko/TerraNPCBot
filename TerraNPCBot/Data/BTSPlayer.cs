﻿using System.Collections.Generic;
using System.IO;
using Terraria;
using TShockAPI;

namespace TerraNPCBot {
    /// <summary>
    /// Bot Terraria Server player, not a seven member South Korean boy band player.
    /// </summary>
    public class BTSPlayer {
        public static BTSPlayer BTSServerPlayer = new BTSPlayer();

        internal BTSPlayer() {
            OwnedBots = new List<Bot>();
            Selected = -1;

            ServerIndex = -1;
        }

        public BTSPlayer(int index) {
            OwnedBots = new List<Bot>();
            Selected = -1;

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

        public uint BotLimit { get; set; } = 5; // Arbitrary limit
        public List<Bot> OwnedBots { get; set; } = new List<Bot>();
        public int Selected { get; set; } = -1;

        public bool autosave = true;
        public bool canBeTeleportedTo = false;
        public bool canBeCopied = false;

        private bool ignoreBots = false;
        public bool IgnoreBots {
            get {
                return ignoreBots;
            }
            set {
                ignoreBots = value;
                ToggleBotVisibility(value);
            }
        }

        private void ToggleBotVisibility(bool visible) {
            if (!visible) {
                foreach (Bot bot in Program.Program.GlobalRunningBots) {
                    // Resend join packets to the player
                    bot.Client.QueuePackets(new Packets.Packet4(bot.PlayerData) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet16(bot.ID, (short)bot.PlayerData.CurHP, (short)bot.PlayerData.MaxHP) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet30(bot.ID, false) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet42(bot.ID, (short)bot.PlayerData.CurMana, (short)bot.PlayerData.MaxMana) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet45(bot.ID, 0) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet50(bot.ID, new byte[22]) { targets = new List<int> { ServerIndex } },
                        new Packets.Packet12(bot.ID, bot.TilePosition) { targets = new List<int> { ServerIndex } });
                }
            }
            else {
                foreach (Bot bot in Program.Program.GlobalRunningBots) {
                    // IgnorePacket bypasses player ignores to tell them bot has disconnected
                    bot.Client.QueuePackets(new Packets.IgnorePacket(bot.ID, false) { targets = new List<int> { ServerIndex } });
                }
            }
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
            get { return TShock.Players[ServerIndex]; }
        }
    }
}
