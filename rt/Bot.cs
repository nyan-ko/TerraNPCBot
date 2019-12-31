using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Terraria;

namespace rt {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        private int _protocol;
        private EventManager _manager;
        public Player _player;
        public Client _client { get; }
        public World _world;

        public int _owner;
        public bool _recording;

        public List<RecordedPacket> _recordedPackets;

        public Stopwatch _timerBetweenPackets;
        public ParsedPacketBase _previousPacket;

        public Bot(string address, int owner, string name = "Michael_Jackson", int port = 7777,  int protocol = 194) {
            _protocol = Main.curRelease;
            _manager = new EventManager();
            {
                _manager._listenReact.Add(Events.Disconnect, Stop);
                _manager._listenReact.Add(Events.ReceivedID, ReceivedPlayerID);
                _manager._listenReact.Add(Events.WorldInfo, Initalize);
            }  // default listeners
            _player = new Player(name);
            _world = new World();
            _owner = owner;
            _client = Client.GetClient(address, this, _player, _world, _manager, port);
        }

        public bool Start() {
            if (_client.Start()) {
                _client.AddPackets(new Packets.Packet1(_protocol));
                return true;
            }
            else
                return false;
        }

        public void Stop(EventInfo i) {
            _client.Stop();
            NetMessage.SendData(2, _player.PlayerID);
        }  // hammer time

        public void ReceivedPlayerID(EventInfo e) {
            _client.AddPackets(new Packets.Packet4(_player));
            _client.AddPackets(new Packets.Packet16(_player));
            _client.AddPackets(new Packets.Packet42(_player));
            _client.AddPackets(new Packets.Packet50(_player));
            for (byte i = 0; i < 83; i++) 
                _client.AddPackets(new Packets.Packet5(_player, i));           
            _client.AddPackets(new Packets.Packet6());
        }

        public void Initalize(EventInfo i) {
            if (_player.Initialized && !_player.LoggedIn) {
                _player.LoggedIn = true;
                _client.AddPackets(new Packets.Packet12(_player.PlayerID));
            }
            if (!_player.Initialized) {
                _player.Initialized = true;
                _client.AddPackets(new Packets.Packet8());
                _client.AddPackets(new Packets.Packet12(_player.PlayerID));
            }
        }
    }
}
