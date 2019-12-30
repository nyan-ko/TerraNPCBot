using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace rt {
    public class PacketBase {

        public uint _packetType;  // type of packet in hex, see multiplayer packet structure
        private List<byte> _data;
        public BinaryWriter Amanuensis;

        public PacketBase(uint packetType, List<byte> data = null) {
            _packetType = packetType;
            _data = data;
        }

        /// <summary>
        /// Encodes and adds data to a packet. Using UTF-8 for strings.
        /// </summary>
        /// <param name="data">ok</param>
        public void EncodeString(string data) {
            byte[] bitsOfBytes = Encoding.UTF8.GetBytes(data);
            var length = (byte)data.Length;
            _data.Add(length);
            _data.AddRange(bitsOfBytes);
        }

        // not big endian compatible yet
        public void AddData(Stream stream) {
            using (MemoryStream s = new MemoryStream()) {
                stream.Position = 0;
                stream.CopyTo(s);
                _data.AddRange(s.ToArray());
            }
        }

        public void Send(Socket socket) {
            try {
                byte[] packet = new byte[_data.Count + 3];
                using (var writer = new BinaryWriter(new MemoryStream(packet))) {
                    writer.Write((short)(_data.Count + 3));
                    writer.Write((byte)_packetType);
                    foreach (var x in _data) {
                        writer.Write(x);
                    }
                }
                socket.Send(packet);
            }
            catch (Exception ex) {
                TShockAPI.TShock.Log.Write($"Exception thrown with Send(Socket): {ex}", System.Diagnostics.TraceLevel.Error);
            }
        }

        // soon
        //public static void LittleEndianifier(byte[] bites) {
            
        //}

        /// <summary>
        /// Parses stream of data into something usable. <para/> Packet structure from https://tshock.readme.io/v4.3.22/docs/multiplayer-packet-structure
        /// </summary>
        /// <param name="reader"></param>
        public static PacketBase Parse(BinaryReader reader, Player plr, World wrld) {
            PacketBase packet = null;

            var length = reader.ReadInt16();
            var type = reader.ReadByte();
            switch (type) {
                case 0x2:  // disconnect
                    var reason = reader.ReadBytes((int)reader.BaseStream.Length);
                    string r = Encoding.UTF8.GetString(reason);
                    packet = new Packets.Packet2(r);
                    break;
                case 0x3:  // continue connection
                    var id = reader.ReadByte();
                    packet = new Packets.Packet3(plr, id);
                    break;
                case 0x7:  // world info
                    packet = new Packets.Packet7(wrld, plr,
                            reader.ReadInt32(),
                            reader.ReadByte(),
                            reader.ReadByte(),
                            reader.ReadInt16(),
                            reader.ReadInt16(),
                            reader.ReadInt16(),
                            reader.ReadInt16());
                    break;
            }
            return packet;
        }
    }
}
