using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class EventManager {
        public Dictionary<PacketTypes, Action<EventInfo>> _listenReact;

        public EventManager() {
            _listenReact = new Dictionary<PacketTypes, Action<EventInfo>>();
        }
    }

    public class EventInfo {
        public Bot b;
        public ParsedPacketBase p;

        public EventInfo(Bot s, ParsedPacketBase e) {
            b = s;
            p = e;
        }
    }

}
