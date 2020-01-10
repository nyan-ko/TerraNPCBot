using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Terraria;
using TShockAPI;

namespace rt {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        private int _protocol;
        public int _owner;
        public bool _recording;
        public bool ActuallyJoined;  //Flag102

        private EventManager _manager;
        public Player _player;
        public Client _client { get; }
        public World _world;

        public List<RecordedPacket> _recordedPackets;
        public Stopwatch _timerBetweenPackets;
        public StreamInfo _previousPacket;

        public Timer _delayBetweenPackets;
        public int _PacketIndex;

        public Bot(string address, int owner, string name = "Michael_Jackson", int port = 7777) {
            _protocol = Main.curRelease;
            _manager = new EventManager();
            {
                _manager._listenReact.Add(PacketTypes.Disconnect, Stop);
                _manager._listenReact.Add(PacketTypes.ContinueConnecting, ReceivedPlayerID);
                _manager._listenReact.Add(PacketTypes.WorldInfo, Initalize);
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
            try {
                NetMessage.SendData(2, ID);
            }
            catch (ArgumentNullException) {
                return;  // if bot has somehow already left the server
            }  
        }  // hammer time

        public void ReceivedPlayerID(EventInfo e) {
            _client.AddPackets(new Packets.Packet4(_player));
            _client.AddPackets(new Packets.Packet16(ID, (short)_player.CurHP, (short)_player.MaxHP));
            _client.AddPackets(new Packets.Packet30(ID, false));
            _client.AddPackets(new Packets.Packet42(ID, (short)_player.CurMana, (short)_player.MaxMana));
            _client.AddPackets(new Packets.Packet45(ID, 0));
            _client.AddPackets(new Packets.Packet50(ID, new byte[22]));

            UpdateInv();

            _client.AddPackets(new Packets.Packet6());
        }

        public void Initalize(EventInfo i) {
            if (_player.Initialized && !_player.LoggedIn) {
                _player.LoggedIn = true;
                _client.AddPackets(new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
            }
            if (!_player.Initialized) {
                _player.Initialized = true;
                _client.AddPackets(new Packets.Packet8());
                _client.AddPackets(new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
            }
        }


        public void RecordedPacketDelay(object sender, ElapsedEventArgs args) {
            var timer = (Timer)sender;
            var currentPacket = _recordedPackets[_PacketIndex];
            bool lastPacket = _PacketIndex == (_recordedPackets.Count - 1);

            if (lastPacket) {
                timer.AutoReset = false;
                timer.Stop();
                timer.Dispose();
                _PacketIndex = 0;
            }
            else {
                timer.Interval = currentPacket.timeBeforeNextPacket;
                ++_PacketIndex;
            }

            var packet = PacketBase.WriteFromRecorded(currentPacket.stream, this);
            if (packet != null)
                _client.AddPackets(packet);
        }

        public void UpdateInv() {
            byte i = 0;
            foreach (var current in _player.InventorySlots) {
                _client.AddPackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.ArmorSlots) {
                _client.AddPackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.DyeSlots) {
                _client.AddPackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.MiscEquipSlots) {
                _client.AddPackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.MiscDyeSlots) {
                _client.AddPackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
        }

        public void ServerInvCopy(Terraria.Player target) {
            Main.ServerSideCharacter = true;
            NetMessage.SendData(7, ID, -1);

            ShallowInvCopy(target);
            UpdateInv();

            Main.ServerSideCharacter = false;
            NetMessage.SendData(7, ID, -1);
        }

        public void ShallowInvCopy(Terraria.Player target) {
            target.inventory.CopyTo(_player.InventorySlots, 0);
            target.armor.CopyTo(_player.ArmorSlots, 0);
            target.dye.CopyTo(_player.DyeSlots, 0);
            target.miscDyes.CopyTo(_player.MiscDyeSlots, 0);
            target.miscEquips.CopyTo(_player.MiscEquipSlots, 0);
        }

        public void PlayerInfoCopy(Terraria.Player target) {
            _player.HairType = (byte)target.hair;
            _player.HairDye = target.hairDye;
            _player.SkinVariant = (byte)target.skinVariant;
            _player.HMisc = target.hideMisc;

            _player.EyeColor = target.eyeColor;
            _player.HairColor = target.hairColor;
            _player.PantsColor = target.pantsColor;
            _player.ShirtColor = target.shirtColor;
            _player.ShoeColor = target.shoeColor;
            _player.SkinColor = target.skinColor;
            _player.UnderShirtColor = target.underShirtColor;

            BitsByte bit1 = 0;
            for (int i = 0; i < 8; ++i) {
                bit1[i] = target.hideVisual[i];
            }
            _player.HVisuals1 = bit1;

            BitsByte bit2 = 0;
            for (int i = 8; i < 10; ++i) {
                bit2[i] = target.hideVisual[i];
            }
            _player.HVisuals2 = bit2;

            if (Running)
                _client.AddPackets(new Packets.Packet4(_player));
        }


        public byte ID {
            get { return _player.PlayerID; }
        }

        public string Name {
            get { return _player.Name; }
        }

        public bool Running {
            get { return _client._running; }
        }

        public TShockAPI.TSPlayer AsTSPlayer {
            get { return TShockAPI.TShock.Players[ID]; }
        }
    }
}
