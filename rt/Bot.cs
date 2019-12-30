using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace rt {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        private int _protocol;
        private EventManager _manager;
        private Player _player;
        private Client _client;
        private World _world;

        public Bot(string address, int port = 7777, string name = "Michael_Jackson", int protocol = 194) {
            _protocol = Main.curRelease;
            _manager = new EventManager();
            {
                _manager._listenReact.Add(Events.Disconnect, Stop);
                _manager._listenReact.Add(Events.ReceivedID, ReceivedPlayerID);
                _manager._listenReact.Add(Events.WorldInfo, Initalize);
            }  // default listeners
            _player = new Player(name);
            _world = new World();
            _client = Client.GetClient(address, this, _player, _world, _manager, port);
        }

        public void Start() {
            _client.Start();
            _client.AddPackets(new Packets.Packet1(_protocol));
        }

        public void Stop(Bot b, PacketBase p) {
            _client.Stop();
        }  // hammer time

        public void ReceivedPlayerID(Bot b, PacketBase p) {
            _client.AddPackets(new Packets.Packet4(_player));
            _client.AddPackets(new Packets.Packet16(_player));
            _client.AddPackets(new Packets.Packet42(_player));
            _client.AddPackets(new Packets.Packet50(_player));
            for (byte i = 0; i < 83; i++) 
                _client.AddPackets(new Packets.Packet5(_player, i));           
            _client.AddPackets(new Packets.Packet6());
        }

        public void Initalize(Bot b, PacketBase p) {
            if (_player.Initialized && !_player.LoggedIn) {
                _player.LoggedIn = true;
                _client.AddPackets(new Packets.Packet12(_player.PlayerID));
            }
            if (!_player.Initialized) {
                _player.Initialized = true;
                _client.AddPackets(new Packets.Packet8());
            }
        }
    }
}
