using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace rt {
    /// <summary>
    /// Bot Terraria Server player, not a seven member South Korean boy band player.
    /// </summary>
    public class BTSPlayer {
        public BTSPlayer(int index) {
            _ownedBots = new List<Bot>();
            _selected = -1;
            _isBot = false;

            ind = index;
        }

        private int ind;
        public uint _botLimit = 10;
        public List<Bot> _ownedBots;
        public int _selected;
        public bool _isBot;

        public Bot SelectedBot {
            get { return _ownedBots.Count > 0 && _selected != -1 ? _ownedBots[_selected] : null; }
        }

        public TSPlayer SPlayer {
            get { return TShock.Players[ind]; }
        }
    }
}
