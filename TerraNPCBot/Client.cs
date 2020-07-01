using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Net.Sockets;
using System.Net;
using Terraria;
using System.Net.Sockets;
using System.Threading;
using TerraBotLib;
using TerraBotLib.Events;

namespace TerraNPCBot {
    /// <summary>
    /// Manages connections for the bot.
    /// </summary>
    public class Client : IClient {
        #region Fields
        public bool running;

        private Bot bot;
#endregion

        public Client (Bot _bot) {
            bot = _bot;
            CanSendPackets = true;
        }

        public void Initialize() {
            bot.EventHooks.ClientStart.Register(InternalStart, HandlerPriority.BelowNormal);
            bot.EventHooks.ClientStart.Register(SendJoinPackets, HandlerPriority.Low);
            bot.EventHooks.ClientDirectedStart.Register(DirectJoinPackets);
            bot.EventHooks.ClientStop.Register(InternalStop);
        }

        public void DirectJoinPackets(object source, StartEventArgs args) {
            byte id = bot.ID;
            int target = args.WhoAsked;
            QueuePackets(new Packets.Packet4(bot.PlayerData).AddTargets(target),
                new Packets.Packet16(id, (short)bot.PlayerData.CurHP, (short)bot.PlayerData.MaxHP).AddTargets(target),
                new Packets.Packet30(id, false).AddTargets(target),
                new Packets.Packet42(id, (short)bot.PlayerData.CurMana, (short)bot.PlayerData.MaxMana).AddTargets(target),
                new Packets.Packet45(id, 0).AddTargets(target),
                new Packets.Packet50(id, new ushort[22]).AddTargets(target),
                new Packets.Packet12(id, (short)bot.TilePosition.X, (short)bot.TilePosition.Y, 0, (byte)PlayerSpawnContext.SpawningIntoWorld).AddTargets(target));

            short i = 0;
            foreach (var current in bot.PlayerData.InventorySlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId).AddTargets(target));
                ++i;
            }
            foreach (var current in bot.PlayerData.ArmorSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId).AddTargets(target));
                ++i;
            }
            foreach (var current in bot.PlayerData.DyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId).AddTargets(target));
                ++i;
            }
            foreach (var current in bot.PlayerData.MiscEquipSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId).AddTargets(target));
                ++i;
            }
            foreach (var current in bot.PlayerData.MiscDyeSlots) {
                QueuePackets(new Packets.Packet5(bot.ID,
                    i,
                    (short)current.Stack,
                    current.PrefixId,
                    (short)current.NetId).AddTargets(target));
                ++i;
            }
        }

        private void SendJoinPackets(object source, StartEventArgs args) {
            byte id = bot.ID;
            QueuePackets(new Packets.Packet4(bot.PlayerData),
                new Packets.Packet16(id, (short)bot.PlayerData.CurHP, (short)bot.PlayerData.MaxHP),
                new Packets.Packet30(id, false),
                new Packets.Packet42(id, (short)bot.PlayerData.CurMana, (short)bot.PlayerData.MaxMana),
                new Packets.Packet45(id, 0),
                new Packets.Packet50(id, new ushort[22]),
                new Packets.Packet12(id, (short)bot.TilePosition.X, (short)bot.TilePosition.Y, 0, (byte)PlayerSpawnContext.SpawningIntoWorld));
            bot.Actions.UpdateInventory();
        }

        private int FindOpenSlot() {
            for (byte index = 254; index > 0; --index) {
                if (Program.PluginMain.Bots[index] == null)
                    return index;
            }
            return -1;
        }

        private void InternalStart(object source, StartEventArgs args) {
            if (!running) {
                int slot = FindOpenSlot();
                if (slot == -1)
                    return;
                running = true;
                bot.ID = (byte)slot;

                #region Slots
                Main.player[slot] = new Terraria.Player();
                TShockAPI.TShock.Players[slot] = new TShockAPI.TSPlayer(slot);
                Program.PluginMain.Bots[slot] = bot;
                #endregion
            }
        }

        private void InternalStop(object source, StopEventArgs args) {
            running = false;
            Program.PluginMain.Bots[bot.ID] = null;
        }

        public bool ToggleSendPackets() {
            CanSendPackets = !CanSendPackets;
            return CanSendPackets;
        }

        public void QueuePackets(params IPacket[] packets) {
            foreach (var packet in packets) {
                packet.Sender = this;
                PacketBase.AddPacket(packet);
            }
        }

        public void Start() {
            bot.EventHooks.ClientStart.Invoke(bot, new StartEventArgs() { WhoAsked = bot.Owner });
        }

        public void Stop() {
            bot.EventHooks.ClientStop.Invoke(bot, new StopEventArgs() { WhoAsked = bot.Owner });
        }

        public bool CanSendPackets { get; set; } = false;

        public bool Running => running;
    }
}
