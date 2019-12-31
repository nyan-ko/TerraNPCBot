using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace rt {
    public class PacketBase {

        public uint _packetType;
        private List<byte> _data;
        public BinaryWriter Amanuensis;

        public PacketBase(uint packetType, List<byte> data = null) {
            _packetType = packetType;
            _data = data;
        }

        /// <summary>
        /// Adds data to a packet from a stream.
        /// </summary>
        /// <param name="stream"></param>
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

        // soon ?
        //public static void LittleEndianifier(byte[] bites) {
            
        //}

        /// <summary>
        /// Parses stream of data into something usable. <para/> Packet structure from https://tshock.readme.io/v4.3.22/docs/multiplayer-packet-structure
        /// </summary>
        /// <param name="reader"></param>
        public static ParsedPacketBase Parse(BinaryReader reader, Player plr, World wrld, GetDataEventArgs args = null) {
            ParsedPacketBase packet = null;

            short length;
            byte type;

            using (reader) {
                if (args == null) {
                    length = reader.ReadInt16();
                    type = reader.ReadByte();
                }
                else {
                    length = (short)args.Length;
                    type = (byte)args.MsgID;
                }

                // Flag102
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
                        packet = new Packets.Packet7(reader, wrld, plr);
                        break;
                    case 0x11: // player update
                        packet = new Packets.Packet13Parser(reader);
                        break;
                }
            }

            return packet;
        }
    }

    public class ParsedPacketBase {
        public uint _packetType;

        public ParsedPacketBase(uint packet) {
            _packetType = packet;
        }

        public static PacketBase Write(object p) {
            PacketBase packet = null;

            try {
                var basePacket = (PacketBase)p;

                switch (basePacket._packetType) {
                    case 0x11: {
                            var temp = (Packets.Packet13Parser)p;
                            packet = new Packets.Packet13(temp.id,
                                temp.control,
                                temp.pulley,
                                temp.selected,
                                temp.posX,
                                temp.posY,
                                temp.vecX,
                                temp.vecY);
                        }
                        break;
                }  // Flag102
            }
            catch (Exception ex) {
                TShockAPI.TShock.Log.Write($"Exception thrown with writing packet from parsed data: {ex.ToString()}", System.Diagnostics.TraceLevel.Error);
                return null;
            }

            return packet;
        }
    }
}
