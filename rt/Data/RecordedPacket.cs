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
