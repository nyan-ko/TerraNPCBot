using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.DatabaseItems {
    public class DBPlayer {
        public List<DBBot> OwnedBots;
        public int BotLimit;
        public bool Teleportable;
        public bool Copyable;
        public bool Ignoring;

        private DBPlayer() { }

        public static DBPlayer ConvertPlayer(BTSPlayer plr) {
            List<DBBot> bots = new List<DBBot>();
            foreach (var bot in plr.OwnedBots) {
                bots.Add(DBBot.ConvertBot(bot));
            }
            return new DBPlayer { BotLimit = plr.BotLimit, Teleportable = plr.CanBeTeleportedTo, Copyable = plr.CanBeCopied, Ignoring = plr.IgnoreBots, OwnedBots = bots };
        }

        public BTSPlayer ConvertDBItem(int index) {
            return new BTSPlayer(this, index);
        }
    }
}
