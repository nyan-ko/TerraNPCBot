using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using TerrariaApi.Server;
using Terraria;

namespace rt {
    public class PacketBase {

        public uint _packetType;
        private List<byte> _data;
        protected BinaryWriter Amanuensis;

        public PacketBase(uint packetType, List<byte> data) {
            _packetType = packetType;
            _data = data;
        }

        /// <summary>
        /// Adds data to a packet from a stream.
        /// </summary>
        /// <param name="stream"></param>
        protected void AddData(Stream stream) {
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
                    case 2:  // disconnect
                        var reason = reader.ReadBytes((int)reader.BaseStream.Length);
                        string r = Encoding.UTF8.GetString(reason);
                        packet = new Packets.Packet2(r);
                        break;
                    case 3:  // continue connection
                        var id = reader.ReadByte();
                        packet = new Packets.Packet3(plr, id);
                        break;
                    case 7:  // world info
                        packet = new Packets.Packet7(reader, wrld, plr);
                        break;
                    case 13: // player update
                        packet = new Packets.Packet13Parser(reader);
                        break;
                }
            }

            return packet;
        }

        public static PacketBase WriteFromRecorded(StreamInfo r, Bot b) {
            PacketBase packet = null;


            using (var reader = new BinaryReader(new MemoryStream(r.Buffer))) {
                try {
                    switch (r.Type) {
                        case 5: {
                                reader.BaseStream.Position += 1;
                                var slot = reader.ReadByte();
                                var stack = reader.ReadInt16();
                                var prefix = reader.ReadByte();
                                var id = reader.ReadInt16();

                                packet = new Packets.Packet5(b.ID, slot, stack, prefix, id);
                            }
                            break;  // inv slot
                        case 12: {
                                reader.BaseStream.Position += 1;
                                var spawnx = reader.ReadInt16();
                                var spawny = reader.ReadInt16();

                                packet = new Packets.Packet12(b.ID, spawnx, spawny);
                            }
                            break;  // player spawn
                        case 13: {
                                reader.BaseStream.Position += 1;
                                var num1 = reader.ReadByte();
                                var num2 = reader.ReadByte();
                                var num3 = reader.ReadByte();
                                var num4 = reader.ReadSingle();
                                var num5 = reader.ReadSingle();
                                float num6 = 0.0F;
                                float num7 = 0.0F;
                                if ((num2 & 4) == 4) {
                                    num6 = reader.ReadSingle();
                                    num7 = reader.ReadSingle();
                                }  // update velocity

                                packet = new Packets.Packet13(b.ID, num1, num2, num3, num4, num5, num6, num7);
                            }
                            break;  // player update
                        case 16: {
                                reader.BaseStream.Position += 1;
                                var hp = reader.ReadInt16();
                                var max = reader.ReadInt16();
                                packet = new Packets.Packet16(b.ID, hp, max);
                            }
                            break;  // player hp
                        case 17: {
                                var action = reader.ReadByte();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var var1 = reader.ReadInt16();
                                var var2 = reader.ReadByte();

                                packet = new Packets.Packet17(action, x, y, var1, var2);
                            }
                            break;  // door toggle
                        case 19: {
                                var action = reader.ReadByte();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var dir = reader.ReadByte();

                                packet = new Packets.Packet19(action, x, y, dir);
                            }
                            break;
                        case 30: {
                                reader.BaseStream.Position += 1;
                                var pvp = reader.ReadBoolean();

                                packet = new Packets.Packet30(b.ID, pvp);
                            }
                            break;
                        

                    }  // Flag102
                }
                catch (Exception ex) {
                    TShockAPI.TShock.Log.Write($"Exception thrown with writing packet from parsed data: {ex.ToString()}", System.Diagnostics.TraceLevel.Error);
                    return packet;
                }
            }
            return packet;
        }
    }
    
    //replace fields with a stream Flag102
    public class ParsedPacketBase {
        public uint _packetType;

        public ParsedPacketBase(uint packet) {
            _packetType = packet;
        }        
    }
}
