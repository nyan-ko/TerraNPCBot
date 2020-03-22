using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using TShockAPI;

namespace TerraNPCBot {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot {
        public const int Protocol = 194;
        public const string Address = "127.0.0.1";

        public int _owner;
      
        public bool _recording;
        public bool _actuallyJoined;

        public Player _player;
        public Client _client;
        public BotActions Actions;

        private Timer heartBeat;

        #region Record Packets
        public bool _playingBack;
        public List<RecordedPacket> _recordedPackets;

        public Stopwatch _timerBetweenPackets;
        public StreamInfo _previousPacket;   
        public Timer _delayBetweenPackets;
        public int _PacketIndex;
        #endregion

        internal Bot() {

        }

        public Bot(int owner) {
            _owner = owner;
            Actions = new BotActions(this);
            _actuallyJoined = false;
        }

        public bool Start() { 
            if (_client.Start()) {
                Program.Program.GlobalRunningBots.Add(this);
                SendJoinPackets();
                return true;
            }
            else
                return false;
        }

        public void Shutdown() {

            _client.Stop();
            Program.Program.GlobalRunningBots.Remove(this);
        }

        #region Default Listeners
        //SHUT UP SHUT UP shut up
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [Obsolete]
        public async Task ReceivedPlayerID(EventPacketInfo unused = null) {
            _client.QueuePackets(new Packets.Packet4(_player));
            _client.QueuePackets(new Packets.Packet16(ID, (short)_player.CurHP, (short)_player.MaxHP));
            _client.QueuePackets(new Packets.Packet30(ID, false));
            _client.QueuePackets(new Packets.Packet42(ID, (short)_player.CurMana, (short)_player.MaxMana));
            _client.QueuePackets(new Packets.Packet45(ID, 0));
            _client.QueuePackets(new Packets.Packet50(ID, new byte[22]));

            UpdateInventory();

            _client.QueuePackets(new Packets.Packet6());
        }

        [Obsolete]
        public async Task AlertAndInfo(EventPacketInfo unused = null) {
            TShock.Players[_owner]?.SendInfoMessage($"Received player id: {ID}");
            Program.Program.Bots[ID] = this;
        }

        [Obsolete]
        public async Task Initialize(EventPacketInfo unused = null) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            _client.QueuePackets(new Packets.Packet8());
            _client.QueuePackets(new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
        }
        #endregion

        [Obsolete]
        private void SendAlive(object sender, ElapsedEventArgs args) {
            if (!ShouldSendAlive())
                return;
            _client.QueuePackets(new Packets.Packet13(ID, 0, 0, (byte)AsTSPlayer.TPlayer.selectedItem, AsTSPlayer.LastNetPosition.X, AsTSPlayer.LastNetPosition.Y));
        }

        private bool ShouldSendAlive() => Running && _actuallyJoined && !_playingBack;
        
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
                _client.QueuePackets(packet);
        }

        public void SendJoinPackets() {
            //_client.AddPackets(new Packets.Packet1(194));
            //await Task.Delay(25);
            _client.QueuePackets(new Packets.Packet4(_player));
            //_client.AddPackets(new Packets.Packet68());
            _client.QueuePackets(new Packets.Packet16(ID, (short)_player.CurHP, (short)_player.MaxHP));
            _client.QueuePackets(new Packets.Packet30(ID, false));
            _client.QueuePackets(new Packets.Packet42(ID, (short)_player.CurMana, (short)_player.MaxMana));
            _client.QueuePackets(new Packets.Packet45(ID, 0));
            _client.QueuePackets(new Packets.Packet50(ID, new byte[22]));
            UpdateInventory();
            //_client.AddPackets(new Packets.Packet6());
            //await Task.Delay(25);
            //_client.AddPackets(new Packets.Packet8());
            _client.QueuePackets(new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
        }

        public void UpdateInventory() {
            byte i = 0;
            foreach (var current in _player.InventorySlots) {
                _client.QueuePackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.ArmorSlots) {
                _client.QueuePackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.DyeSlots) {
                _client.QueuePackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.MiscEquipSlots) {
                _client.QueuePackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
            foreach (var current in _player.MiscDyeSlots) {
                _client.QueuePackets(new Packets.Packet5(ID,
                    i,
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
                ++i;
            }
        }
  
        public byte ID {
            get { return (byte)_player.PlayerID; }
            set => _player.PlayerID = value;
        }

        /// <summary>
        /// Gets the bot's player name.
        /// </summary>
        public string Name => _player.Name;

        /// <summary>
        /// Gets bot's running status.
        /// </summary>
        public bool Running => _client._running;

        /// <summary>
        /// Gets the TSPlayer associated with the bot's index.
        /// </summary>
        public TSPlayer AsTSPlayer => TShock.Players[ID];

        /// <summary>
        /// Gets the bot's timer to send PlayerUpdate (13) periodically to prevent time-out; potentially obsolete for new optimized bots.
        /// </summary>
        public Timer _heartBeat => heartBeat;
    }

    public class BotActions {
        private Bot bot;

        public BotActions(Bot _bot) {
            bot = _bot;
        }

        public void Teleport(Microsoft.Xna.Framework.Vector2 pos) {
            bot._client.QueuePackets(new Packets.Packet13(bot.ID, 0, 0, (byte)bot.AsTSPlayer.TPlayer.selectedItem, pos.X, pos.Y));
        }

        public void Chat(string message) {
            bot._client.QueuePackets(new Packets.Packet82(message));
        }

        /// <summary>
        /// Copies target's inventory and player info.
        /// </summary>
        /// <param name="target">The target as a Terraria.Player.</param>
        public void FullCopy(Terraria.Player target) {
            InventoryCopy(target);
            InfoCopy(target);
        }

        /// <summary>
        /// Copies target's inventory and updates bot's inventory to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public void InventoryCopy(Terraria.Player target) {
            var _player = bot._player;
            for (int i = 0; i < NetItem.InventorySlots; ++i) {
                _player.InventorySlots[i] = ItemData.FromTerrariaItem(target.inventory[i]);
            }
            for (int i = 0; i < NetItem.ArmorSlots; ++i) {
                _player.ArmorSlots[i] = ItemData.FromTerrariaItem(target.armor[i]);
            }
            for (int i = 0; i < NetItem.DyeSlots; ++i) {
                _player.DyeSlots[i] = ItemData.FromTerrariaItem(target.dye[i]);
            }
            for (int i = 0; i < NetItem.MiscDyeSlots; ++i) {
                _player.MiscDyeSlots[i] = ItemData.FromTerrariaItem(target.miscDyes[i]);
            }
            for (int i = 0; i < NetItem.MiscEquipSlots; ++i) {
                _player.MiscEquipSlots[i] = ItemData.FromTerrariaItem(target.miscEquips[i]);
            }
            if (bot.Running)
                bot.UpdateInventory();
        }

        /// <summary>
        /// Copies target's player info and updates bot's info to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public void InfoCopy(Terraria.Player target) {
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

            // Terraria uses this struct so I'm doing it too
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
                bot._client.QueuePackets(new Packets.Packet4(_player));
        }
    }

}
