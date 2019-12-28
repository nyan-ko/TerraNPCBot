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

        public void Move() {

        }
    }
    public enum Events {
        Disconnect = 0x2,
        ReceivedID = 0x3,
        WorldInfo = 0x8
    }

}
