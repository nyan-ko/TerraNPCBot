using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class RecordedPacket {
        public ParsedPacketBase packet;
        public int timeBeforeNextPacket;

        public RecordedPacket(ParsedPacketBase p, int i) {
            packet = p;
            timeBeforeNextPacket = i;
        }
    }
}
