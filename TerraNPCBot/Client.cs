using System;
using System.IO;
using System.Collections.Generic;
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
        public const string UUID = "bigchungusisfunnybigchungusisfunnybigchungusisfunnybigchungusisfunnybigchungusisfunnybigchungusisfunnybigchungusisfunnybigchungu";
        public bool _running;
        public int _port;

        private Bot _bot;
        private Thread _writeThread;
        private List<PacketBase> _writeQueue;

        public Client (int port, Bot bot) {
            _bot = bot;
            _port = port;

            _writeThread = new Thread(SendPackets);
            _writeThread.IsBackground = true;

            _writeQueue = new List<PacketBase>();
        }

        public void QueuePackets (PacketBase packet) {
            _writeQueue.Add(packet);
        }

        private void SendPackets() { 
            while (_running) {
                if (_writeQueue.Count > 0) {
                    try {
                        _writeQueue[0].Send();
                        _writeQueue.RemoveAt(0);
                    }
                    catch {
                        
                    }
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

        public bool Start() {
            if (!_writeThread.IsAlive) {
                try {
                    int slot = FindOpenSlot();
                    if (slot == -1)
                        return false;
                    _running = true;
                    _bot.ID = (byte)slot;
                    _writeThread.Start();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Exception thrown while getting server info: {ex.ToString()} {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown while getting server info (Start()): {ex.ToString()} {ex.Source}", System.Diagnostics.TraceLevel.Error);

                    return false;
                }
                return _running;
            }
            Console.WriteLine($"Exception thrown while getting server info: One or more threads active.");
            TShockAPI.TShock.Log.Write($"Exception thrown while getting server info: One or more threads active.", System.Diagnostics.TraceLevel.Error);

            return false;
        }

        /// <summary>
        /// Stops the bot.
        /// </summary>
        public void Stop() {
            _running = false;
        }

        //public static ISocket[] ClientSockets = new TcpSocket[256];

        //public static void UnusedClientWriteCallback(object unused) {
        //    //Lol!
        //}
    }

    [Obsolete]
    public class BotSocket : ISocket {
        private Bot bot;

        public BotSocket(Bot b) : base() {
            bot = b;
        }

        #region Nobody really cares.
        public void Connect(RemoteAddress address) {
            //Console.WriteLine("Connect");
        }

        public RemoteAddress GetRemoteAddress() {
            //Console.WriteLine("GetRemoteAddress");
            return null;
        }

        public bool IsDataAvailable() {
            //Console.WriteLine("IsDataAvailable");
            return false;
        }

        public void SendQueuedPackets() {
            //Console.WriteLine("SendQueuedPackets");
        }

        public bool StartListening(SocketConnectionAccepted callback) {
            //Console.WriteLine("StartListening");
            return false;
        }

        public void StopListening() {
            //Console.WriteLine("StopListening");
        }

        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state) {
            //Console.WriteLine("AsyncReceive");
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state) {
            //Console.WriteLine("AsyncSend");
        }
        #endregion

        public bool IsConnected() {
            return false;
        }

        void ISocket.Close() {
            //bot.AsTSPlayer?.Disconnect("Bot disconnected.");
        }
    }
}
