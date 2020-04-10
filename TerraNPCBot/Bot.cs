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

        public int owner;
      
        public bool recording;
        public bool actuallyJoined;

        //public EventManager manager;
        public Player player;
        public Client client;
        public BotActions Actions;

        private Timer heartBeat;

        #region Record Packets
        public bool playingBack;
        public List<RecordedPacket> recordedPackets;

        public Stopwatch timerBetweenPackets;
        public StreamInfo previousPacket;   
        public Timer delayBetweenPackets;
        public int PacketIndex;
        #endregion

        internal Bot() {

        }

        public Bot(int _owner, int ownedBotsIndex) {
            owner = _owner;
            Actions = new BotActions(this);
            IndexInOwnerBots = ownedBotsIndex;

            heartBeat = new Timer(8000);
            heartBeat.Elapsed += SendAlive;
            heartBeat.AutoReset = true;
        }

        public bool Start() { 
            if (client.Start()) {
                Program.Program.GlobalRunningBots.Add(this);
                SendJoinPackets();
                heartBeat.Start();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Sends a player inactive packet to all players then tells itself to stop its write thread.
        /// </summary>
        public void Shutdown() {
            QueuePackets(new Packets.Packet14(ID, false),
                new Packets.ShutdownPacket());
            Program.Program.GlobalRunningBots.Remove(this);
        }

        #region Record
        public void StartRecordTimer() {
            delayBetweenPackets = new Timer(10);
            delayBetweenPackets.Elapsed += RecordedPacketDelay;
            delayBetweenPackets.AutoReset = true;
            delayBetweenPackets.Start();
            playingBack = true;
        }

        private void RecordedPacketDelay(object sender, ElapsedEventArgs args) {
            var timer = (Timer)sender;
            var currentPacket = recordedPackets[PacketIndex];
            bool lastPacket = PacketIndex == (recordedPackets.Count - 1);

            if (lastPacket) {
                timer.Stop();
                timer.Dispose();
                PacketIndex = 0;
                playingBack = false;
            }
            else {
                timer.Interval = currentPacket.timeBeforeNextPacket == 0 ? currentPacket.timeBeforeNextPacket + 1 : currentPacket.timeBeforeNextPacket;
                ++PacketIndex;
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

        private bool ShouldSendAlive => Running && !playingBack;

        private void QueuePackets(params PacketBase[] packets) => client.QueuePackets(packets);

        private void SendJoinPackets() {
            QueuePackets(new Packets.Packet4(player),
                new Packets.Packet16(ID, (short)player.CurHP, (short)player.MaxHP),
                new Packets.Packet30(ID, false),
                new Packets.Packet42(ID, (short)player.CurMana, (short)player.MaxMana),
                new Packets.Packet45(ID, 0),
                new Packets.Packet50(ID, new byte[22]),
                new Packets.Packet12(ID, (short)Main.spawnTileX, (short)Main.spawnTileY));
            Actions.UpdateInventory();
        }

        #region Properties
        public byte ID {
            get {
                if (Running)
                    return (byte)player.PlayerID;
                return 0;
            }
            set => player.PlayerID = value;
        }

        /// <summary>
        /// Gets the bot's player name.
        /// </summary>
        public string Name => player.Name;

        /// <summary>
        /// Gets the bot's position in the owner's owned bots.
        /// </summary>
        public int IndexInOwnerBots { get; set; }

        /// <summary>
        /// Gets the bot's running status.
        /// </summary>
        public bool Running => client.running;

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

        public BotActions(Bot b) {
            bot = b;
        }

        private void QueuePackets(params PacketBase[] packets) => bot.client.QueuePackets(packets);

        public virtual void UpdateInventory() {
            byte i = 0;
            foreach (var current in bot.player.InventorySlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot.player.ArmorSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot.player.DyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot.player.MiscEquipSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    current.stack,
                    current.prefix,
                    current.netID));
                ++i;
            }
            foreach (var current in bot.player.MiscDyeSlots) {
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
            var player = bot.player;
            for (int i = 0; i < NetItem.InventorySlots; ++i) {
                player.InventorySlots[i] = ItemData.FromTerrariaItem(target.inventory[i]);
            }
            for (int i = 0; i < NetItem.ArmorSlots; ++i) {
                player.ArmorSlots[i] = ItemData.FromTerrariaItem(target.armor[i]);
            }
            for (int i = 0; i < NetItem.DyeSlots; ++i) {
                player.DyeSlots[i] = ItemData.FromTerrariaItem(target.dye[i]);
            }
            for (int i = 0; i < NetItem.MiscDyeSlots; ++i) {
                player.MiscDyeSlots[i] = ItemData.FromTerrariaItem(target.miscDyes[i]);
            }
            for (int i = 0; i < NetItem.MiscEquipSlots; ++i) {
                player.MiscEquipSlots[i] = ItemData.FromTerrariaItem(target.miscEquips[i]);
            }
            if (bot.Running)
                UpdateInventory();
        }

        /// <summary>
        /// Copies target's player info and updates bot's info to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public virtual void InfoCopy(Terraria.Player target) {
            var player = bot.player;
            player.HairType = (byte)target.hair;
            player.HairDye = target.hairDye;
            player.SkinVariant = (byte)target.skinVariant;
            player.HMisc = target.hideMisc;

            player.EyeColor = target.eyeColor;
            player.HairColor = target.hairColor;
            player.PantsColor = target.pantsColor;
            player.ShirtColor = target.shirtColor;
            player.ShoeColor = target.shoeColor;
            player.SkinColor = target.skinColor;
            player.UnderShirtColor = target.underShirtColor;

            // Terraria uses this struct so I'm doing it too
            BitsByte bit1 = 0;
            for (int i = 0; i < 8; ++i) {
                bit1[i] = target.hideVisual[i];
            }
            player.HVisuals1 = bit1;
            BitsByte bit2 = 0;
            for (int i = 8; i < 10; ++i) {
                bit2[i] = target.hideVisual[i];
            }
            player.HVisuals2 = bit2;

            if (bot.Running)
                QueuePackets(new Packets.Packet4(player));
        }
    }

}
