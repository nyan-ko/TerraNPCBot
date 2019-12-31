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
        }

        public int _botLimit;
        public List<Bot> _ownedBots;
        public int _selected;

        public Bot SelectedBot {
            get { return _ownedBots[_selected]; }
        }
    }
}
