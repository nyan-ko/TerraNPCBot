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
            _ownedBots = new List<Bot>();
            _selected = -1;
        }

        public BTSPlayer(int index) {
            _ownedBots = new List<Bot>();
            _selected = -1;

            _serverIndex = index;
        }

        private int _serverIndex;
        public uint _botLimit = 10; //Flag102
        public List<Bot> _ownedBots;
        public int _selected;
        public int _selectedDelete {
            get {
                int del = _selectedDelete;
                _selectedDelete = -1;
                return del;
            }
            set { _selectedDelete = value; }
        }

        public bool _autosave;
        public bool _canBeTeleportedTo;
        public bool _canBeCopied;

        public List<Bot> GetBotFromIndexOrName(string nameOrIndex) {
            List<Bot> found = new List<Bot>();

            if (int.TryParse(nameOrIndex, out int index)) {
                if (index < _ownedBots.Count && index >= 0)
                    return new List<Bot> { _ownedBots[index] };
                else {
                    return found;
                }
            }

            foreach (Bot bot in _ownedBots) {
                if (bot.Name.ToLower() == nameOrIndex)
                    return new List<Bot> { bot };
                if (bot.Name.ToLower().StartsWith(nameOrIndex))
                    found.Add(bot);
            }

            return found;
        }

        public Bot SelectedBot {
            get { return _ownedBots.Count > 0 && _selected != -1 ? _ownedBots[_selected] : null; }
        }

        public TSPlayer SPlayer {
            get { return TShock.Players[_serverIndex]; }
        }
    }
}
