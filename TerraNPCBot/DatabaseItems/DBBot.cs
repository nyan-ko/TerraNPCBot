using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.DatabaseItems {
    public class DBBot {
        public Player PlayerData;
        public Vector2 Position;
        public int Port;

        private DBBot() { }

        public static DBBot ConvertBot(Bot bot) {
            return new DBBot { PlayerData = bot.PlayerData, Position = bot.Position, Port = bot.Port };
        }

        public Bot ConvertDBItem(int owner, int indexInOwnedBots) {
            return new Bot(this, owner, indexInOwnedBots);
        }
    }
}
