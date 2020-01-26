using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using TShockAPI;

namespace rt {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        public const int Protocol = 194;
        public const string Address = "127.0.0.1";

        public int _owner;
        public bool _recording;
        public bool _actuallyJoined;

        internal EventManager _manager;
        public Player _player;
        public Client _client { get; }
        public World _world;
        public BotActions Actions;

        private Timer heartBeat;
        public Timer _checkJoin;

        #region Record Packets
        public bool _playingBack;
        public List<RecordedPacket> _recordedPackets;

        public Stopwatch _timerBetweenPackets;
        public StreamInfo _previousPacket;   
        public Timer _delayBetweenPackets;
        public int _PacketIndex;
        #endregion


        public Bot(string address, int owner, int port = 7777, string name = "Michael_Jackson") {
            _manager = new EventManager();
            {
                _manager._listenReact.Add(PacketTypes.ContinueConnecting, new ParallelTask(ReceivedPlayerID, AlertAndInfo));
                _manager._listenReact.Add(PacketTypes.WorldInfo, new ParallelTask(Initialize));
            }  // default listeners
            _player = new Player(name);
            _world = new World();
            _owner = owner;
            _client = new Client(address, this, _player, _world, _manager, port);
            Actions = new BotActions(this);

            heartBeat = new Timer(15000);
            heartBeat.Elapsed += SendAlive;
            heartBeat.AutoReset = true;
            heartBeat.Start();

            _actuallyJoined = false;

            _checkJoin = new Timer(9000);
            _checkJoin.AutoReset = true;
            _checkJoin.Elapsed += CheckForJoin;
        }

        public bool Start() { 
            if (_client.Start()) {
                _client.AddPackets(new Packets.Packet1(Protocol));
                return true;
            }
            else
                return false;
        }

        public async void Stop() {
            _client.Stop();
            await Task.Delay(2);
            _client.DisconnectAndReuse();
        }

        //SHUT UP SHUT UP shut up
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ReceivedPlayerID(EventPacketInfo e) {
            _client.AddPackets(new Packets.Packet4(_player));
            _client.AddPackets(new Packets.Packet16(ID, (short)_player.CurHP, (short)_player.MaxHP));
            _client.AddPackets(new Packets.Packet30(ID, false));
            _client.AddPackets(new Packets.Packet42(ID, (short)_player.CurMana, (short)_player.MaxMana));
            _client.AddPackets(new Packets.Packet45(ID, 0));
            _client.AddPackets(new Packets.Packet50(ID, new byte[22]));

            UpdateInv();

            _client.AddPackets(new Packets.Packet6());
        }

        public async Task AlertAndInfo(EventPacketInfo e) {
            Program.Program.Players[_owner].SPlayer.SendInfoMessage($"Recieved player id: {ID}");
            Program.Program.Bots[ID] = this;
        }

        public async Task Initialize(EventPacketInfo i) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            _client.AddPackets(new Packets.Packet8());
            _client.AddPackets(new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
        }

        private void SendAlive(object sender, ElapsedEventArgs args) {
            if (!ShouldSend())
                return;
            _client.AddPackets(new Packets.Packet13(ID, 0, 0, (byte)AsTSPlayer.TPlayer.selectedItem, AsTSPlayer.LastNetPosition.X, AsTSPlayer.LastNetPosition.Y));
        }

        private bool ShouldSend() {
            return !(!Running || _playingBack || !_actuallyJoined);
        }
        
        public void StartRecordTimer() {
            _delayBetweenPackets = new Timer(10);
            _delayBetweenPackets.Elapsed += RecordedPacketDelay;
            _delayBetweenPackets.AutoReset = true;
            _delayBetweenPackets.Start();
            _playingBack = true;
        }

        public void RecordedPacketDelay(object sender, ElapsedEventArgs args) {
            var timer = (Timer)sender;
            var currentPacket = _recordedPackets[_PacketIndex];
            bool lastPacket = _PacketIndex == (_recordedPackets.Count - 1);

            if (lastPacket) {
                timer.Stop();
                timer.Dispose();
                _PacketIndex = 0;
                _playingBack = false;
            }
            else {
                timer.Interval = currentPacket.timeBeforeNextPacket == 0 ? currentPacket.timeBeforeNextPacket + 1 : currentPacket.timeBeforeNextPacket;
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

            for (byte x = i; x < NetItem.MaxInventory; ++x) {
                _client.AddPackets(new Packets.Packet5(ID,
                    x,
                    0,
                    0,
                    0));
            }
        }

        public async void CheckForJoin(object sender, ElapsedEventArgs args) {
            if (!_actuallyJoined) {
                Stop();
                await Task.Delay(10);
                Start();
                TShock.Players[_owner].SendInfoMessage("Retrying connection...");
            }
            else {
                _checkJoin.Stop();
            }
        }
  
        public byte ID {
            get { return (byte)_player.PlayerID; }
            set { _player.PlayerID = _player.PlayerID == -1 ? value : _player.PlayerID; }
        }

        public string Name {
            get { return _player.Name; }
        }

        public bool Running {
            get { return _client._running; }
        }

        public TSPlayer AsTSPlayer {
            get { return TShock.Players[ID]; }
        }
    }

    public class BotActions {
        private Bot bot;

        public BotActions(Bot _bot) {
            bot = _bot;
        }

        public void Teleport(Microsoft.Xna.Framework.Vector2 pos) {
            bot._client.AddPackets(new Packets.Packet13(bot.ID, 0, 0, (byte)bot.AsTSPlayer.TPlayer.selectedItem, pos.X, pos.Y));
        }

        public void Chat(string message) {
            bot._client.AddPackets(new Packets.Packet82(message));
        }

        public void Copy(Terraria.Player target) {
            if (bot.Running)
                ServerInvCopy(target);
            else
                ShallowInvCopy(target);
            PlayerInfoCopy(target);
        }

        public void ServerInvCopy(Terraria.Player target) {
            ShallowInvCopy(target);
            bot.UpdateInv();
        }

        public void ShallowInvCopy(Terraria.Player target) {
            var _player = bot._player;
            target.inventory.CopyTo(_player.InventorySlots, 0);
            target.armor.CopyTo(_player.ArmorSlots, 0);
            target.dye.CopyTo(_player.DyeSlots, 0);
            target.miscDyes.CopyTo(_player.MiscDyeSlots, 0);
            target.miscEquips.CopyTo(_player.MiscEquipSlots, 0);
        }

        public void PlayerInfoCopy(Terraria.Player target) {
            var _player = bot._player;
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

            if (bot.Running)
                bot._client.AddPackets(new Packets.Packet4(_player));
        }
    }

}
