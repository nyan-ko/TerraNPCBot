﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using TerraBotLib;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Timers;
using Terraria;
using TShockAPI;
using TerraBotLib.Events;

namespace TerraNPCBot {
    /// <summary>
    /// Comprises bot info and functions.
    /// </summary>
    public class Bot : IBot {
        #region Fields
        #region Interface Fields
        private Client client;
            #endregion
        #region Record Packets
            public bool recording;

            public bool playingBack;
            public List<RecordedPacket> recordedPackets;

            public Stopwatch timerBetweenPackets;
            public StreamInfo previousPacket;
            private Timer delayBetweenPackets;
            private int packetIndex;
        #endregion

        private int owner;
        private int indexInOwnerBots;
        private Vector2 position;
        private Timer heartBeat;
        #endregion
        #region Properties
        #region Interface Properties
        public Hooks EventHooks { get; private set; } = new Hooks();
        public IClient Client => client;
        public AdditionalBotData AdditionalData { get; private set; } = new AdditionalBotData();
        #endregion

        public Player PlayerData { get; private set; }
        public BotActions Actions { get; private set; }

        /// <summary>
        /// Gets or sets the bot's server index.
        /// </summary>
        public byte ID {
            get => (byte)PlayerData.Index;
            set => PlayerData.Index = value;
        }

        /// <summary>
        /// Gets or sets the bot's player name.
        /// </summary>
        public string Name {
            get => PlayerData.Name;
            set {
                if (EventHooks.NameChange.Invoke(this, new NameChangedEventArgs() { OldName = PlayerData?.Name, NewName = value }))
                    return;
                PlayerData.Name = value;
                if (Running)
                    QueuePackets(new Packets.Packet4(PlayerData));
            }
        }

        /// <summary>
        /// Gets or sets the bot's owner.
        /// </summary>
        public int Owner {
            get => owner;
            set {
                if (EventHooks.OwnerChange.Invoke(this, new OwnerChangedEventArgs() { OldOwner = owner, NewOwner = value }))
                    return;
                owner = value;
            }
        }

        /// <summary>
        /// Gets or sets the bot's position in the owner's owned bots.
        /// </summary>
        public int IndexInOwnerBots {
            get => indexInOwnerBots;
            set {
                if (EventHooks.OwnedIndexChange.Invoke(this, new OwnerIndexChangedEventArgs() { OldIndex = indexInOwnerBots, NewIndex = value }))
                    return;
                indexInOwnerBots = value;
            }
        }

        /// <summary>
        /// Gets the bot's running status.
        /// </summary>
        public bool Running => client.running;

        /// <summary>
        /// Gets or sets the bot's position on the server.
        /// </summary>
        public Vector2 Position {
            get => position;
            set {
                if (EventHooks.PositionChange.Invoke(this, new PositionChangedEventArgs() { OldPosition = position, NewPosition = value }))
                    return;
                position = value;

                if (Running)
                    QueuePackets(new Packets.Packet13(ID, 0, 0, 0, 0, (byte)AsTSPlayer.TPlayer.selectedItem, position.X, position.Y));
            }
        }

        public Vector2 TilePosition => Position / 16;

        /// <summary>
        /// Gets the TSPlayer associated with the bot's index.
        /// </summary>
        public TSPlayer AsTSPlayer => TShock.Players[ID];

        /// <summary>
        /// Gets the bot's chat color based off its group on the server.
        /// </summary>
        public Color ChatColor => new Color(AsTSPlayer.Group.R, AsTSPlayer.Group.G, AsTSPlayer.Group.B);
        #endregion

        internal Bot(DatabaseItems.DBBot dbBot, int _owner, int indexInOwnedBots) {
            owner = _owner;
            client = new Client(this);
            Actions = new BotActions(this);
            EventHooks = new Hooks();

            position = dbBot.Position;
            PlayerData = dbBot.PlayerData;
            IndexInOwnerBots = indexInOwnedBots;

            SetupHeartbeat();

            Initialize();
        }

        public Bot(string name, int _owner, int ownedBotsIndex) {
            owner = _owner;
            client = new Client(this);
            Actions = new BotActions(this);
            EventHooks = new Hooks();

            PlayerData = new Player(name, this);
            position = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            indexInOwnerBots = ownedBotsIndex;

            SetupHeartbeat();

            Initialize();

        }

        public void Initialize() {
            if (EventHooks.BotCreate.Invoke(this, new CreateBotEventArgs() { WhoAsked = Owner, Bot = this }))
                return;

            client.Initialize();
        }

        private void SetupHeartbeat() {
            heartBeat = new Timer(90000); // 1 minute 30 seconds, all players time out after 2 minutes
            heartBeat.Elapsed += SendAlive;
            heartBeat.AutoReset = true;
        }

        public bool Start() {
            Client.Start();
            if (Running)
                Program.PluginMain.GlobalRunningBots.Add(this);
            return Running;
        }

        /// <summary>
        /// Sends a player inactive packet to all players then tells itself to stop running.
        /// </summary>
        public void Shutdown() {
            QueuePackets(new Packets.Packet14(ID, false),
                new Packets.ShutdownPacket());
            Program.PluginMain.GlobalRunningBots.Remove(this);
        }

        #region Record
        public void StartPlaybackTimer() {
            delayBetweenPackets = new Timer(10);
            delayBetweenPackets.Elapsed += RecordedPacketDelay;
            delayBetweenPackets.AutoReset = true;
            delayBetweenPackets.Start();
            playingBack = true;
        }

        private void RecordedPacketDelay(object sender, ElapsedEventArgs args) {
            var timer = (Timer)sender;
            var currentPacket = recordedPackets[packetIndex];
            bool lastPacket = packetIndex == (recordedPackets.Count - 1);

            if (lastPacket) {
                timer.Stop();
                timer.Dispose();
                packetIndex = 0;
                playingBack = false;
            }
            else {
                timer.Interval = currentPacket.timeBeforeNextPacket == 0 ? 1 : currentPacket.timeBeforeNextPacket;
                ++packetIndex;
            }

            var packet = RecordedPacket.WriteFromRecorded(currentPacket.stream, this);
            if (packet != null)
                QueuePackets(packet);
        }
        #endregion

        private void SendAlive(object source, ElapsedEventArgs args) {
            if (!ShouldSendAlive)
                return;
            QueuePackets(new Packets.Packet13(ID, 0, 0, 0, 0, (byte)AsTSPlayer.TPlayer.selectedItem, Position.X, Position.Y));
        }

        private bool ShouldSendAlive => Running && !playingBack;

        private void QueuePackets(params IPacket[] packets) => client.QueuePackets(packets);

        public void RequestJoinPackets(int whoAsked) {
            EventHooks.ClientDirectedStart.Invoke(this, new StartEventArgs() { Bot = this, WhoAsked = whoAsked });
        }

        public override string ToString() => Name;
    }

    public class BotActions {
        private Bot bot;

        public BotActions(Bot b) {
            bot = b;
        }

        private void QueuePackets(params IPacket[] packets) => bot.Client.QueuePackets(packets);

        public virtual void UpdateInventory() {
            short i = 0;
            foreach (var current in bot.PlayerData.InventorySlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId));
                ++i;
            }
            foreach (var current in bot.PlayerData.ArmorSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId));
                ++i;
            }
            foreach (var current in bot.PlayerData.DyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId));
                ++i;
            }
            foreach (var current in bot.PlayerData.MiscEquipSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId));
                ++i;
            }
            foreach (var current in bot.PlayerData.MiscDyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId));
                ++i;
            }
        }

        public virtual void Teleport(Microsoft.Xna.Framework.Vector2 pos) => bot.Position = pos;
        
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
            var player = bot.PlayerData;
            for (int i = 0; i < NetItem.InventorySlots; ++i) {
                player.InventorySlots[i] = (NetItem)target.inventory[i];
            }
            for (int i = 0; i < NetItem.ArmorSlots; ++i) {
                player.ArmorSlots[i] = (NetItem)target.armor[i];
            }
            for (int i = 0; i < NetItem.DyeSlots; ++i) {
                player.DyeSlots[i] = (NetItem)target.dye[i];
            }
            for (int i = 0; i < NetItem.MiscDyeSlots; ++i) {
                player.MiscDyeSlots[i] = (NetItem)target.miscDyes[i];
            }
            for (int i = 0; i < NetItem.MiscEquipSlots; ++i) {
                player.MiscEquipSlots[i] = (NetItem)target.miscEquips[i];
            }
            if (bot.Running)
                UpdateInventory();
        }

        /// <summary>
        /// Copies target's player info and updates bot's info to match if it is running.
        /// </summary>
        /// <param name="target"></param>
        public virtual void InfoCopy(Terraria.Player target) {
            var player = bot.PlayerData;
            player.HairType = (byte)target.hair;
            player.HairDye = target.hairDye;
            player.SkinVariant = (byte)target.skinVariant;
            player.HideMisc = target.hideMisc;

            player.EyeColor = target.eyeColor;
            player.HairColor = target.hairColor;
            player.PantsColor = target.pantsColor;
            player.ShirtColor = target.shirtColor;
            player.ShoeColor = target.shoeColor;
            player.SkinColor = target.skinColor;
            player.UndershirtColor = target.underShirtColor;

            // Terraria uses this struct so I'm doing it too
            BitsByte bit1 = 0;
            for (int i = 0; i < 8; ++i) {
                bit1[i] = target.hideVisibleAccessory[i];
            }
            player.HideVisuals = bit1;
            BitsByte bit2 = 0;
            for (int i = 8; i < 10; ++i) {
                bit2[i] = target.hideVisibleAccessory[i];
            }
            player.HideVisuals2 = bit2;

            if (bot.Running)
                QueuePackets(new Packets.Packet4(bot.PlayerData));
        }
    }

}
