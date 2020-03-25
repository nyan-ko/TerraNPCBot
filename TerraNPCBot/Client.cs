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
        public bool _running;
        public int _port;

        private Bot _bot;
        private Thread _writeThread;
        public bool _sendPackets;
        private BlockingCollection<PacketBase> _writeQueue;

        public Client (int port, Bot bot) {
            _bot = bot;
            _port = port;

            _writeQueue = new BlockingCollection<PacketBase>();
        }

        public void QueuePackets (PacketBase packet) {
            _writeQueue.Add(packet);
        }

        private void SendPackets() { 
            while (_running) {
                try {
                    if (!_writeQueue.TryTake(out PacketBase packet) || !_sendPackets) {
                        break; 
                    }
                    packet.Send();
                }
                catch (OperationCanceledException) {
                    break;
                }
            }
        }         

        private int FindOpenSlot() {
            for (byte index = 0; index < Main.maxNetPlayers; ++index) {
                if (!Netplay.Clients[index].IsConnected())
                    return index;
            }
            return -1;
        }

        /// <summary>
        /// Allocates an index and starts the packet writing thread.
        /// </summary>
        /// <returns></returns>
        public bool Start() {
            if (!_writeThread.IsAlive) {
                try {
                    int slot = FindOpenSlot();
                    if (slot == -1)
                        return false;
                    _running = true;
                    _bot.ID = (byte)slot;
                    Netplay.Clients[slot].Reset();
                    Netplay.Clients[slot].Socket = new ConnectedFillerSocket();
                    _writeThread = new Thread(SendPackets);
                    _writeThread.IsBackground = true;
                    _writeThread.Start();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Exception thrown while getting server info: {ex.ToString()} {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown while getting server info (Start()): {ex.ToString()} {ex.Source}", System.Diagnostics.TraceLevel.Error);

                    return false;
                }
                return _running;
            }
            return false;
        }

        /// <summary>
        /// Stops the bot's write thread.
        /// </summary>
        public void Stop() {
            _running = false;
        }
    }

    /// <summary>
    /// Takes up a slot in the players array to prevent index overlap.
    /// </summary>
    public class ConnectedFillerSocket : ISocket {
        public bool IsConnected() {
            return true;
        }

        #region Useless
        public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null) {
        }

        public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null) {
        }

        public void Close() {
        }

        public void Connect(RemoteAddress address) {
        }

        public RemoteAddress GetRemoteAddress() {
            return null;
        }



        public bool IsDataAvailable() {
            return false;
        }

        public void SendQueuedPackets() {
        }

        public bool StartListening(SocketConnectionAccepted callback) {
            return false;
        }

        public void StopListening() {
        }
        #endregion
    }
}
