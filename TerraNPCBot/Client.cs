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
        public int port;

        private Bot bot;
        private bool sendPackets;
#endregion

        public Client (int _port, Bot _bot) {
            bot = _bot;
            port = _port;
            CanSendPackets = true;
            //writeThread = new Thread(SendPackets);
            //writeThread.IsBackground = true;

            //writeQueue = new BlockingCollection<IPacket>();
        }

        public void Initialize() {
            bot.EventHooks.ClientStart.Register(InternalStart, HandlerPriority.BelowNormal);
            bot.EventHooks.ClientStart.Register(SendJoinPackets, HandlerPriority.Low);
            bot.EventHooks.ClientStop.Register(InternalStop);
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

        //private void SendPackets() { 
        //    while (running) {
        //        try {
        //            if (!writeQueue.TryTake(out IPacket packet, -1))
        //                continue;
                   
        //            if (packet.Type == (byte)ExtendedPacketTypes.Shutdown) {
        //                // Plugin-exclusive shutdown packet, must follow a player active packet
        //                // with an inactive flag to both disconnect the bot and stop this write thread
        //                Stop();
        //                break;
        //            }
        //            if (!sendPackets)
        //                continue;
        //            packet.Send();
        //        }
        //        catch (OperationCanceledException) {
        //            continue;
        //        }
        //    }
        //}         

        private int FindOpenSlot() {
            for (byte index = 254; index > 0; --index) {
                if (Program.Program.Bots[index] == null)
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
                Program.Program.Bots[slot] = bot;
                #endregion
            }
        }

        private void InternalStop(object source, StopEventArgs args) {
            running = false;
            Program.Program.Bots[bot.ID] = null;
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
