using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace rt {
    public class Client {

        /// <summary>
        /// Manages connections for the bot.
        /// </summary>
        string _address;
        int _port;
        public bool _running;

        Player _player;
        World _world;
        public EventManager _eventManager;

        Thread _writeThread;
        Thread _readThread;
        byte[] _buffer;
        //public Thread _rejoin;

        List<PacketBase> _writeQueue;

        Socket _client;
        Bot _bot;

        public Client (string host, Bot bot, Player plr, World wrld, EventManager eventManager, int port = 7777) {
            _address = host;
            _port = port;
            _player = plr;
            _world = wrld;
            _eventManager = eventManager;

            _bot = bot;

            _writeThread = new Thread(SendPackets);
            _writeThread.IsBackground = true;
            _readThread = new Thread(ReadPackets);
            _readThread.IsBackground = true;
            //_rejoin = new Thread(Rejoin);
            //_rejoin.IsBackground = true;

            _writeQueue = new List<PacketBase>();

            _buffer = new byte[1024];
        }

        public void AddPackets (PacketBase packet) {
            _writeQueue.Add(packet);
        }

        public void ReadPackets() {
            while (_running) {
                try {
                    var bytes = _client.Receive(_buffer); 
                    byte[] stream = new byte[bytes];
                    Array.Copy(_buffer, stream, bytes);
                    using (var reader = new BinaryReader(new MemoryStream(stream))) {
                        var packedPacket = PacketBase.Parse(reader, _player, _world, _bot);
                        if (packedPacket == null) return;
                        try {
                            _eventManager._listenReact[(PacketTypes)packedPacket._packetType].Invoke(new EventPacketInfo(_bot, packedPacket));
                        }
                        catch { }
                    }
                    _buffer = new byte[1024];
                }
                catch (Exception ex){
                    Console.WriteLine($"Exception thrown when reading packet: {ex}, {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown when reading packet: {ex}, {ex.Source}", System.Diagnostics.TraceLevel.Error);
                }
            }
        }

        public void SendPackets() { 
            while (_running) {
                if (_writeQueue.Count > 0) {
                    try {
                        _writeQueue[0].Send(_client);
                        _writeQueue.RemoveAt(0);
                    }
                    catch {
                        continue;
                    }
                }
            }
        } 

        //public void Rejoin() {
        //    Stop();
        //    while (_writeThread.IsAlive || _readThread.IsAlive) {
        //        continue;
        //    }
        //    Start();
        //}

        /// <summary>
        /// Connects to the server. <para/>Using https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
        /// </summary>
        public bool Start() {
            if (!_writeThread.IsAlive && !_readThread.IsAlive) {
                try {
                    IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress address = IPAddress.Parse(_address);
                    IPEndPoint endPoint = new IPEndPoint(address, _port);

                    _client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    try {
                        _client.Connect(endPoint);
                        _running = true;
                        _writeThread.Start();
                        _readThread.Start();
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Exception thrown while connecting to the server: {ex.ToString()}");
                        TShockAPI.TShock.Log.Write($"Exception thrown while connecting to the server: {ex.ToString()}", System.Diagnostics.TraceLevel.Error);

                        return false;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Exception thrown while getting server info: {ex.ToString()}");
                    TShockAPI.TShock.Log.Write($"Exception thrown while getting server info (Start()): {ex.ToString()}", System.Diagnostics.TraceLevel.Error);

                    return false;
                }
                return _client.Connected;
            }
            return false;
        }

        /// <summary>
        /// Stops the bot.
        /// </summary>
        public void Stop() {
            _running = false;
                     
            _writeThread = new Thread(SendPackets);
            _writeThread.IsBackground = true;
            _readThread = new Thread(ReadPackets);
            _readThread.IsBackground = true;
        }

        public void Disconnect() {
            _client.Shutdown(SocketShutdown.Both);
            _client.Disconnect(true);
        }
    }
}
