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
using Terraria.Net;

namespace TerraNPCBot {
    /// <summary>
    /// Manages connections for the bot.
    /// </summary>
    public class Client {
        public const int BufferSize = 131072;       
        public bool running;
        public int port;

        private Bot bot;
        private Thread writeThread;
        public bool sendPackets;
        //private static CancellationTokenSource cancelToken = new CancellationTokenSource();
        private BlockingCollection<PacketBase> writeQueue;

        public Client (int _port, Bot _bot) {
            bot = _bot;
            port = _port;
            sendPackets = true;
            writeThread = new Thread(SendPackets);
            writeThread.IsBackground = true;

            writeQueue = new BlockingCollection<PacketBase>();
        }

        public void QueuePackets (params PacketBase[] packets) {
            foreach (var packet in packets) {
                writeQueue.Add(packet);
            }
        }

        private void SendPackets() { 
            while (running) {
                try {
                    if (!writeQueue.TryTake(out PacketBase packet, -1))
                        continue; 
                    if (packet.packetType == 255) {
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
        public bool Start() {
            if (!writeThread.IsAlive) {
                try {
                    int slot = FindOpenSlot();
                    if (slot == -1)
                        return false;
                    running = true;
                    bot.ID = (byte)slot;

                    #region Slots
                    Main.player[slot] = new Terraria.Player();
                    TShockAPI.TShock.Players[slot] = new TShockAPI.TSPlayer(slot);
                    Program.Program.Bots[slot] = bot;
                    #endregion

                    writeThread.Start();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Exception thrown while getting server info: {ex.ToString()} {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown while getting server info (Start()): {ex.ToString()} {ex.Source}", System.Diagnostics.TraceLevel.Error);

                    return false;
                }
                return running;
            }
            return false;
        }

        /// <summary>
        /// Stops the bot's write thread.
        /// </summary>
        public async void Stop() {
            running = false;
            await Task.Delay(500);
            writeThread = new Thread(SendPackets);
            writeThread.IsBackground = true;
        }

        public bool ToggleSendPackets() {
            sendPackets = !sendPackets;
            return sendPackets;
        }
    }
}
