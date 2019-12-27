using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        private int _protocol;
        private EventManager _manager;
        private Player _player;
        private Client _client;

        public Bot(string address, string name = "Michael_Jackson", int port = 7777, int protocol = 194) {
            _protocol = protocol;
            _manager = new EventManager();
            _player = new Player(name);
            _client = Client.GetClient(address, _player, _manager, port);


        }

        public void BotGetData(PacketBase packet) {

        }
    }
}
