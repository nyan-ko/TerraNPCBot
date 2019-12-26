using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class PacketBase {

        public uint _packetType;  // type of packet in hex, see multiplayer packet structure
        public List<byte> _data;  

        public PacketBase(uint packetType, List<byte> data) {
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
                TShockAPI.TShock.Log.Write($"'AddData' failed. Could not get bytes from data: {ex}", System.Diagnostics.TraceLevel.Error);
                return;
            }

            if (pascalString) {
                int length = data.Length;
                try {
                    _data.AddRange(BitConverter.GetBytes((byte)length));
                    _data.AddRange(bitsOfBytes);
                }
                catch (ArgumentNullException ex) {
                    TShockAPI.TShock.Log.Write($"'AddData' failed. {ex}, {ex.Source}", System.Diagnostics.TraceLevel.Error);
                    return;
                }
            }
            else {
                _data.AddRange(bitsOfBytes);
            }
        }

        // not big endian compatible yet
        public void AddStructuredData<T> (int data) {
            var tempData = ConvertFromType<T>(data);
            if (tempData == null) return;
            _data.AddRange(tempData);
        }

        /// <summary>
        /// Equivalent method to struct.pack() in python.
        /// </summary>
        /// <typeparam name="T">Type to cast data as.</typeparam>
        /// <param name="data">What do you think this is?</param>
        /// <returns></returns>
        public byte[] ConvertFromType<T> (int data) {
            byte[] dada = null;
            switch (Type.GetTypeCode(typeof(T))) {
                case TypeCode.UInt32:  // long?
                    dada = BitConverter.GetBytes((uint)data);
                    break;
                case TypeCode.UInt16:  // short?
                    dada = BitConverter.GetBytes((ushort)data);
                    break;
                case TypeCode.Byte:  // char?
                    dada = BitConverter.GetBytes((byte)data);
                    break;
                case TypeCode.SByte:  // alt char
                    dada = BitConverter.GetBytes((sbyte)data);
                    break;
            }
            return dada == null ? null : dada;
        }

        public void LittleEndianifier(byte[] bites) {
            
        }
    }
}
