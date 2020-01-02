using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace rt {
    public class RecordedPacket {
        public StreamInfo reader;
        public int packetType;
        public int timeBeforeNextPacket;

        public RecordedPacket(StreamInfo p, int i, int i2) {
            reader = p;
            timeBeforeNextPacket = i;
            packetType = i2;
        }

        public override string ToString() {
            string s ="";

            s += "Packet: " + (PacketTypes)packetType;
            s += $", {timeBeforeNextPacket} milliseconds until next packet.";

            return s;
        }
    }

    public class StreamInfo {
        public byte[] Buffer;
        public int Index;
        public int Length;

        public StreamInfo(byte[] n1, int n2, int n3) {
            Buffer = n1;
            Index = n2;
            Length = n3;
        }
    }
}
