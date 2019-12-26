using System;
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
        EventManager _eventManager;

        Thread _writeThread;
        Thread _readThread;

        List<PacketBase> _writeQueue;

        public static Client GetClient(string host, Player plr, EventManager eventManager, int port = 7777) {

            Client c = new Client();
            c._address = host;
            c._port = port;
            c._player = plr;
            c._eventManager = eventManager;

            c._writeQueue = new List<PacketBase>();

            c._writeThread = new Thread(c.AddPackets);
            c._writeThread.IsBackground = true;

            c._readThread = new Thread(c.ReadPackets);
            c._readThread.IsBackground = true;

            return c;
        }

        public void AddPackets (object obj) {

            PacketBase packet;
            try {
                packet = (PacketBase)obj;
            }
            catch (InvalidCastException) {
                return;
            }
            _writeQueue.Add(packet);            
        }

        public void ReadPackets() {
            while (_running) {

            }
        } // more code needed

        public void SendPackets() { 
            while (_running) {
                if (_writeQueue.Count > 0) {
                    
                }
            }
        }  // python uses socket send()

        /// <summary>
        /// Connects to the server.
        /// </summary>
        public void Start() {
            if (!_writeThread.IsAlive && !_readThread.IsAlive) {

                IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress address = IPAddress.Parse(_address);
                IPEndPoint endPoint = new IPEndPoint(address, _port);

                var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try {

                    client.Connect(endPoint);
                }
                catch {

                }
            }
        }
    }
}
