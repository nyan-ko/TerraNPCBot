﻿using System;
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
        bool _running;

        Player _player;
        World _world;
        EventManager _eventManager;

        Thread _writeThread;
        Thread _readThread;
        byte[] _buffer;

        List<PacketBase> _writeQueue;

        Socket client;

        Bot bot;

        public static Client GetClient(string host, Bot bot, Player plr, World wrld, EventManager eventManager, int port = 7777) {
            Client c = new Client();
            c._address = host;
            c._port = port;
            c._player = plr;
            c._world = wrld;
            c._eventManager = eventManager;

            c._writeQueue = new List<PacketBase>();

            c._buffer = new byte[1024];

            c._writeThread = new Thread(c.SendPackets);
            c._writeThread.IsBackground = true;

            c._readThread = new Thread(c.ReadPackets);
            c._readThread.IsBackground = true;

            // threads are background threads so they stop when program stops
            return c;
        }

        public void AddPackets (PacketBase packet) {
            _writeQueue.Add(packet);
        }

        public void ReadPackets() {
            while (_running) {
                try {
                    var bytes = client.Receive(_buffer);  // blocked until data is received
                    byte[] stream = new byte[bytes];
                    Array.Copy(_buffer, stream, bytes);
                    using (var reader = new BinaryReader(new MemoryStream(stream))) {
                        var packedPacket = PacketBase.Parse(reader, _player, _world);
                        if (packedPacket == null) return;
                        try {
                            _eventManager._listenReact[(Events)packedPacket._packetType].Invoke(bot, packedPacket);
                        }
                        catch { return; }
                    }
                }
                catch (Exception ex){
                    Console.WriteLine($"Exception thrown when reading packet: {ex}, {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown when reading packet: {ex}, {ex.Source}", System.Diagnostics.TraceLevel.Error);
                }
            }
        } // more code needed

        public void SendPackets() { 
            while (_running) {
                if (_writeQueue.Count > 0) {
                    _writeQueue[0].Send(client);
                    _writeQueue.RemoveAt(0);
                    _writeQueue.TrimExcess();  // just in case
                    Thread.Sleep(1);
                }
            }
        } 

        /// <summary>
        /// Connects to the server. <para/>Using https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example
        /// </summary>
        public void Start() {
            if (!_writeThread.IsAlive && !_readThread.IsAlive) {
                try {
                    IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress address = IPAddress.Parse(_address);
                    IPEndPoint endPoint = new IPEndPoint(address, _port);

                    client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    try {
                        client.Connect(endPoint);
                        _running = true;
                        _writeThread.Start();
                        _readThread.Start();
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Exception thrown while connecting to the server: {ex.ToString()}");
                        TShockAPI.TShock.Log.Write($"Exception thrown while connecting to the server: {ex.ToString()}", System.Diagnostics.TraceLevel.Error);

                        return;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Exception thrown while getting server info: {ex.ToString()}");
                    TShockAPI.TShock.Log.Write($"Exception thrown while getting server info (Start()): {ex.ToString()}", System.Diagnostics.TraceLevel.Error);

                    return;
                }
            }
        }

        /// <summary>
        /// Stops the bot.
        /// </summary>
        public void Stop() {
            _running = false;
        }
    }
}
