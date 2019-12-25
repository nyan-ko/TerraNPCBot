using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class PacketBase {
        public uint _packetType;
        public List<byte> _data;

        public PacketBase(uint packetType, List<byte> data = null) {

        }

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
                    _data.Add((byte)length);
                    _data.AddRange(bitsOfBytes);
                }
                catch (InvalidCastException ex) {
                    TShockAPI.TShock.Log.Write($"'AddData' failed. Length could not be cast as byte: {ex}", System.Diagnostics.TraceLevel.Error);
                    return;
                }
            }
            else {
                _data.AddRange(bitsOfBytes);
            }
        }

        public void AddStructuredData(string type, int data) {

        }
    }
}
