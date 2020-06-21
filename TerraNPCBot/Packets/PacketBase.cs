using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria.Net.Sockets;
using TShockAPI;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using TerrariaApi.Server;
using System.Collections.Concurrent;
using TerraBotLib;
using TerraNPCBot.Utils;
using Terraria;

namespace TerraNPCBot {
    public class PacketBase : IPacket {
        #region Properties
        public IClient Sender { get; set; }

        public short TotalLength => (short)data.Length;

        public byte Type => packetType;

        public byte[] Data => data;

        #endregion
        #region Fields
        public byte packetType;
        public List<int> targets = new List<int>();

        private byte[] data;
        protected BinaryWriter Amanuensis;
        #endregion

        protected PacketBase(byte _packetType) {
            Amanuensis = new BinaryWriter(new MemoryStream());
            packetType = _packetType;
        }

        /// <summary>
        /// Creates the actual packet from the data stream with a header.
        /// </summary>
        /// <param name="stream"></param>
        protected void Packetize(bool bypass = false) { // I will NOT switch to American English
            using (MemoryStream stream = new MemoryStream()) {
                short packetLength = (short)(Amanuensis.BaseStream.Position + 3);
                Amanuensis.Write(packetLength);
                Amanuensis.Write(Type);
                Amanuensis.BaseStream.Position = 0;
                Amanuensis.BaseStream.CopyTo(stream);
                data = new byte[packetLength];
                byte[] temp = stream.ToArray();

                Buffer.BlockCopy(temp, 0, data, 3, temp.Length - 3); // Copies actual packet data
                Buffer.BlockCopy(temp, packetLength - 3, data, 0, 3); // Copies packet header (length, type)
            }
            if (bypass) {
                if (targets.Count == 0)
                    throw new Exception();
                BypassIgnore();
            }

        }

        public void Send() { 
            // Force sent packet
            // Must be initialized with a target in the 'targets' list
            if (packetType == (byte)ExtendedPacketTypes.ForceSend) { 
                try {
                    foreach (int target in targets) {
                        RemoteClient client = Netplay.Clients[target];
                        client.Socket.AsyncSend(Data, 0, TotalLength, client.ServerWriteCallBack);
                    }
                }
                catch { }

                return;
            }

            // Specific targets
            if (targets.Count != 0) {
                foreach (int i in targets) {
                    if (Program.Program.Players[i]?.IgnoreBots ?? true)
                        return;
                    try {
                        Netplay.Clients[i].Socket.AsyncSend(Data, 0, TotalLength, Netplay.Clients[i].ServerWriteCallBack);
                    }
                    catch { }
                }
                return;
            }

            // Entire server
            for (int i = 0; i < 256; ++i) {
                if (Netplay.Clients[i].IsConnected() && Program.Program.Bots[i] == null && (!Program.Program.Players[i]?.IgnoreBots ?? false)) {
                    try {
                        Netplay.Clients[i].Socket.AsyncSend(Data, 0, TotalLength, Netplay.Clients[i].ServerWriteCallBack);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Changes packet type so it is forcefully sent and bypasses player ignores. Only call after Packetize() pweez
        /// </summary>
        protected void BypassIgnore() {
            packetType = 254;
        }

        private static BlockingCollection<IPacket> packetQueue = new BlockingCollection<IPacket>();

        public static void AddPacket(IPacket packet) => packetQueue.Add(packet);

        internal static void PacketSendThread(object unused) {
            while (true) {
                try {
                    if (!packetQueue.TryTake(out IPacket packet, -1))
                        continue;

                    if (packet.Type == (byte)ExtendedPacketTypes.Shutdown) {
                        // Plugin-exclusive shutdown packet, must follow a player active packet
                        packet.Sender.Stop();
                        continue;
                    }
                    if (!packet.Sender.CanSendPackets)
                        continue;

                    packet.Send();
                }
                catch (OperationCanceledException) {
                    continue;
                }
            }
        }
    }

    public enum ExtendedPacketTypes {
        Shutdown = 255,
        ForceSend = 254

    }
}
