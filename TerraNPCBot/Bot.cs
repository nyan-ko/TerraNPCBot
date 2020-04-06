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

        //public EventManager _manager;
        public Player _player;
        public Client _client;
        public BotActions Actions;

        private Timer _heartBeat;

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

        public Bot(int owner, int ownedBotsIndex) {
            _owner = owner;
            Actions = new BotActions(this);
            IndexInOwnerBots = ownedBotsIndex;

            _heartBeat = new Timer(8000);
            _heartBeat.Elapsed += SendAlive;
            _heartBeat.AutoReset = true;
        }

        public bool Start() { 
            if (_client.Start()) {
                Program.Program.GlobalRunningBots.Add(this);
                SendJoinPackets();
                _heartBeat.Start();
                return true;
            }
            else
                return false;
        }

        public async void Shutdown() {
            QueuePackets(new Packets.Packet14(ID, false));
            await Task.Delay(500);
            _client.Stop();
            Program.Program.GlobalRunningBots.Remove(this);
        }

        #region Record
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
                QueuePackets(packet);
        }
        #endregion

        private void SendAlive(object source, ElapsedEventArgs args) {
            if (!ShouldSendAlive) 
                return;
            
            QueuePackets(new Packets.Packet14(ID, true));
        }

        private bool ShouldSendAlive => Running && !_playingBack;

        private void QueuePackets(params PacketBase[] packets) => _client.QueuePackets(packets);

        private void SendJoinPackets() {
            QueuePackets(new Packets.Packet4(_player),
                new Packets.Packet16(ID, (short)_player.CurHP, (short)_player.MaxHP),
                new Packets.Packet30(ID, false),
                new Packets.Packet42(ID, (short)_player.CurMana, (short)_player.MaxMana),
                new Packets.Packet45(ID, 0),
                new Packets.Packet50(ID, new byte[22]),
                new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
            Actions.UpdateInventory();
        }

        #region Properties
        public byte ID {
            get {
                if (Running)
                    return (byte)_player.PlayerID;
                return 0;
            }
            set => _player.PlayerID = value;
        }

        /// <summary>
        /// Gets the bot's player name.
        /// </summary>
        public string Name => _player.Name;

        /// <summary>
        /// Gets the bot's position in the owner's owned bots.
        /// </summary>
        public int IndexInOwnerBots { get; set; }

        /// <summary>
        /// Gets the bot's running status.
        /// </summary>
        public bool Running => _client._running;

        /// <summary>
        /// Gets the TSPlayer associated with the bot's index.
        /// </summary>
        public TSPlayer AsTSPlayer => TShock.Players[ID];

        /// <summary>
        /// Gets the bot's chat color based off its group on the server.
        /// </summary>
        public Microsoft.Xna.Framework.Color ChatColor => new Microsoft.Xna.Framework.Color(AsTSPlayer.Group.R, AsTSPlayer.Group.G, AsTSPlayer.Group.B);
#endregion
    }

    public class BotActions {
        private Bot bot;

        public BotActions(Bot _bot) {
            bot = _bot;
        }

        private void QueuePackets(params PacketBase[] packets) => bot._client.QueuePackets(packets);

        public virtual void UpdateInventory() {
            byte i = 0;
            foreach (var current in bot._player.InventorySlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot._player.ArmorSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot._player.DyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot._player.MiscEquipSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot._player.MiscDyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
        }

        public virtual void Teleport(Microsoft.Xna.Framework.Vector2 pos) => QueuePackets(new Packets.Packet13(bot.ID, 0, 0, (byte)bot.AsTSPlayer.TPlayer.selectedItem, pos.X, pos.Y));
        
        public virtual void PlayNote(float note) => QueuePackets(new Packets.Packet58(bot.ID, note));

        public virtual void Chat(string message) => QueuePackets(new Packets.Packet82(message, bot.ID, bot.ChatColor));
        
        /// <summary>
        /// Copies target's inventory and player info.
        /// </summary>
        /// <param name="target">The target as a Terraria.Player.</param>
        public virtual void FullCopy(Terraria.Player target) {
            InventoryCopy(target);
            InfoCopy(target);
        }

        /// <summary>
        /// Copies target's inventory and updates bot's inventory to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public virtual void InventoryCopy(Terraria.Player target) {
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
                UpdateInventory();
        }

        /// <summary>
        /// Copies target's player info and updates bot's info to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public virtual void InfoCopy(Terraria.Player target) {
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
                QueuePackets(new Packets.Packet4(_player));
        }
    }

}
