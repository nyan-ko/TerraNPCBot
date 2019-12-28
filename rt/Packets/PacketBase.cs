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

        public PacketBase(uint packetType, List<byte> data = null) {
            _packetType = packetType;
            _data = data;
        }

        /// <summary>
        /// Encodes and adds data to a packet. Using UTF-8.
        /// </summary>
        /// <param name="data">ok</param>
        /// <param name="pascalString">Whether length should be encoded with rest of data.</param>
        public void AddData(string data, bool pascalString = false) {

            byte[] bitsOfBytes;
            try {
                bitsOfBytes = Encoding.UTF8.GetBytes(data);
            }
            catch (ArgumentNullException ex) {
                Console.Write($"'AddData' failed. Could not get bytes from data: {ex}");
                TShockAPI.TShock.Log.Write($"'AddData' failed. Could not get bytes from data: {ex}", System.Diagnostics.TraceLevel.Error);
                return;
            }

            if (pascalString) {
                var length = ConvertToType<short>(data.Length);
                try {
                    _data.AddRange(length);
                    _data.AddRange(bitsOfBytes);
                }
                catch (ArgumentNullException ex) {
                    Console.WriteLine($"Exception with AddData: {ex}, {ex.Source}");
                    TShockAPI.TShock.Log.Write($"Exception thrown with AddData(data, pascalString): {ex}, {ex.Source}", System.Diagnostics.TraceLevel.Error);
                    return;
                }
            }
            else {
                _data.AddRange(bitsOfBytes);
            }
        }

        // not big endian compatible yet
        public void AddStructuredData<T> (float data) {
            var tempData = ConvertToType<T>(data);
            if (tempData == null) return;
            _data.AddRange(tempData);
        }

        public void Send(Socket socket) {
            try {
                var packet = ConvertToType<ushort>(_data.Count + 3);  // adding 3 to include length of packet (2 bytes) and type (1 byte)
                packet.AddRange(ConvertToType<byte>(_packetType));
                packet.AddRange(_data);

                TShockAPI.TShock.Log.Write(_data.Count.ToString(), System.Diagnostics.TraceLevel.Verbose);

                socket.Send(packet.ToArray());
            }
            catch (Exception ex) {
                TShockAPI.TShock.Log.Write($"Exception thrown with Send(Socket): {ex}", System.Diagnostics.TraceLevel.Error);
            }
        }

        /// <summary>
        /// Equivalent method to struct.pack() in python.
        /// </summary>
        /// <typeparam name="T">Type to cast data as.</typeparam>
        /// <param name="data">What do you think this is?</param>
        /// <returns></returns>
        public static List<byte> ConvertToType<T> (float data) {
            List<byte> dada = null;
            switch (Type.GetTypeCode(typeof(T))) {
                case TypeCode.Int32:
                    dada = BitConverter.GetBytes((int)data).ToList();
                    break;
                case TypeCode.UInt32:  // ulong?
                    dada = BitConverter.GetBytes((uint)data).ToList();
                    break;
                case TypeCode.Int16:
                    dada = BitConverter.GetBytes((short)data).ToList();
                    break;
                case TypeCode.UInt16:  // ushort?
                    dada = BitConverter.GetBytes((ushort)data).ToList();
                    break;
                case TypeCode.Byte:  // char?
                    dada = BitConverter.GetBytes((byte)data).ToList();
                    break;
                case TypeCode.SByte:  // alt char
                    dada = BitConverter.GetBytes((sbyte)data).ToList();
                    break;
                case TypeCode.Single:  // float
                    dada = BitConverter.GetBytes((float)data).ToList();
                    break;
            }
            return dada;
        }

        public static void LittleEndianifier(byte[] bites) {
            
        }

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
