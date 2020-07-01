using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TerraNPCBot {
    public class RecordedPacket {
        public StreamInfo stream;
        public uint timeBeforeNextPacket;

        public RecordedPacket(StreamInfo st, uint time) {
            stream = st;
            timeBeforeNextPacket = time;
        }        

        public static PacketBase WriteFromRecorded(StreamInfo r, Bot b) {
            PacketBase packet = null;

            using (var writer = new BinaryWriter(new MemoryStream(r.Buffer))) {
                try {
                    switch (r.Type) {
                        case 5:
                        case 12:
                        case 13:
                        case 16:
                        case 30:
                        case 41:
                        case 42:
                        case 45:
                        case 50:
                        case 55:
                        case 96:
                        case 117:
                        case 118:
                            writer.Write(b.ID);

                            writer.BaseStream.Position = 0;
                            packet = new Packets.RawPacket(r.Type, ((MemoryStream)writer.BaseStream).ToArray());
                            break;
                        case 17: 
                        case 19: 
                        case 65:
                            writer.BaseStream.Position = 0;
                            packet = new Packets.RawPacket(r.Type, ((MemoryStream)writer.BaseStream).ToArray());
                            break;
                    }
                }
                catch (Exception ex) {
                    TShockAPI.TShock.Log.Write($"Exception thrown with writing packet from parsed data: {ex.ToString()}, Bot: {b.Name}", System.Diagnostics.TraceLevel.Error);
                    return null;
                }
            }
            return packet;
        }

    }

    public class StreamInfo {
        public byte[] Buffer;
        public byte Type;

        public StreamInfo(byte[] stream, byte type) {
            Buffer = stream;
            Type = type;
        }
    }
}
