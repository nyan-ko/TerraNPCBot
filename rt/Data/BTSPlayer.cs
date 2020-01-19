using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace rt {
    /// <summary>
    /// Bot Terraria Server player, not a seven member South Korean boy band player.
    /// </summary>
    public class BTSPlayer : TSPlayer {
        public BTSPlayer(int index) : base(index) {
            _ownedBots = new List<Bot>();
            _selected = -1;
            _isBot = false;
        }

        public int _botLimit = 10;
        public List<Bot> _ownedBots;
        public int _selected;
        public bool _isBot;

        private Bot _bot;

        public Bot SelectedBot {
            get { return _ownedBots.Count > 0 && _selected != -1 ? _ownedBots[_selected] : null; }
        }

        public Bot BotPlayer {
            get { return _isBot ? _bot : null; }
            set { _bot = value; }
        }
    }
}
