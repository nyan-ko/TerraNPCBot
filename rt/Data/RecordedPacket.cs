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
        public int timeBeforeNextPacket;

        public RecordedPacket(StreamInfo p, int i) {
            stream = p;
            timeBeforeNextPacket = i;
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
        public int Type;

        public StreamInfo(byte[] n1, int n4) {
            Buffer = n1;
            Type = n4;
        }
    }
}
