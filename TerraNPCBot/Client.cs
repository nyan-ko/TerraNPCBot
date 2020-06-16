using System;
using System.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Net.Sockets;
using System.Net;
using Terraria;
using System.Net.Sockets;
using System.Threading;
using TerraBotLib;
using Terraria.Net;
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
        private Thread writeThread;
        public bool sendPackets;
        private BlockingCollection<IPacket > writeQueue;
#endregion

        public Client (int _port, Bot _bot) {
            bot = _bot;
            port = _port;
            sendPackets = true;
            writeThread = new Thread(SendPackets);
            writeThread.IsBackground = true;

            writeQueue = new BlockingCollection<IPacket>();
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
                new Packets.Packet50(id, new byte[22]),
                new Packets.Packet12(id, bot.TilePosition));
            bot.Actions.UpdateInventory();
        }

        private void SendPackets() { 
            while (running) {
                try {
                    if (!writeQueue.TryTake(out IPacket packet, -1))
                        continue;

                    if (bot.EventHooks.ClientSendPacket.Invoke(bot, new PacketSentEventArgs { Packet = packet, PacketData = packet.Data, Port = port }))
                        return;
                   
                    if (packet.Type == 255) {
                        // Plugin-exclusive shutdown packet, must follow a player active packet
                        // with an inactive flag to both disconnect the bot and stop this write thread
                        Stop();
                        break;
                    }
                    if (!sendPackets)
                        continue;
                    packet.Send();
                }
                catch (OperationCanceledException) {
                    continue;
                }
            }
        }         

        private int FindOpenSlot() {
            for (byte index = 254; index > 0; --index) {
                if (Program.Program.Bots[index] == null)
                    return index;
            }
            return -1;
        }

        /// <summary>
        /// Allocates an index and starts the packet writing thread.
        /// </summary>
        /// <returns></returns>
        private void InternalStart(object source, StartEventArgs args) {
            if (!writeThread.IsAlive) {
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

                writeThread.Start();
            }
        }

        private void InternalStop(object source, StopEventArgs args) {
            running = false;  // stop writeThread's while running loop
            writeThread.Join();  // wait for writeThread to finish

            writeThread = new Thread(SendPackets);
            writeThread.IsBackground = true;
        }

        public bool ToggleSendPackets() {
            sendPackets = !sendPackets;
            return sendPackets;
        }

        public void QueuePackets(params IPacket[] packets) {
            foreach (var packet in packets) {
                writeQueue.Add(packet);
            }
        }

        public void Start() {
            bot.EventHooks.ClientStart.Invoke(bot, new StartEventArgs() { WhoAsked = bot.Owner });
        }

        public void Stop() {
            bot.EventHooks.ClientStop.Invoke(bot, new StopEventArgs() { WhoAsked = bot.Owner });
        }

        public bool CanSendPackets => sendPackets;

        public bool Running => running;
    }
}
