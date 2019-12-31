using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class EventManager {
        public Dictionary<Events, Action<EventInfo>> _listenReact;

        public EventManager() {
            _listenReact = new Dictionary<Events, Action<EventInfo>>();
        }

        public static void Move(Bot b, ParsedPacketBase p) {

        }
    }
    public enum Events {
        Disconnect = 0x2,
        ReceivedID = 0x3,
        WorldInfo = 0x7,

        // more events in accordance with server-sent packets
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
