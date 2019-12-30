using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class EventManager {
        public Dictionary<Events, Action<Bot, PacketBase>> _listenReact;

        public EventManager() {
            _listenReact = new Dictionary<Events, Action<Bot, PacketBase>>();
        }
    }
    public enum Events {
        Disconnect = 0x2,
        ReceivedID = 0x3,
        WorldInfo = 0x7,

        // more events in accordance with server-sent packets
    }

}
