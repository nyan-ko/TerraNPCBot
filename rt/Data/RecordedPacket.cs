using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace rt {
    public class RecordedPacket {
        public StreamInfo stream;
        public uint timeBeforeNextPacket;

        public RecordedPacket(StreamInfo st, uint time) {
            stream = st;
            timeBeforeNextPacket = time;
        }

        public void WriteToStream(BinaryWriter writer) {
            writer.Write(stream.Buffer.Length);
            writer.Write(stream.Buffer);
            writer.Write(stream.Type);
            writer.Write(timeBeforeNextPacket);

        }

        public override string ToString() {
            string s ="";

            s += "Packet: " + (PacketTypes)stream.Type;
            s += $", {timeBeforeNextPacket} milliseconds until next packet.";

            return s;
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
