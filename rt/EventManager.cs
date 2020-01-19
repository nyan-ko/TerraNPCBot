using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt {
    public class EventManager {
        public Dictionary<PacketTypes, ParallelTask> _listenReact;

        public EventManager() {
            _listenReact = new Dictionary<PacketTypes, ParallelTask>();
        }
    }

    public class EventPacketInfo {
        public Bot b;
        public ParsedPacketBase p;

        public EventPacketInfo(Bot s, ParsedPacketBase e) {
            b = s;
            p = e;
        }
    }

    public class ParallelTask {
        public List<Func<EventPacketInfo, Task>> Tasks;

        public ParallelTask(params Func<EventPacketInfo, Task>[] tasks) {
            Tasks = new List<Func<EventPacketInfo, Task>>();
            Tasks.AddRange(tasks);
        }

        public async void Invoke(EventPacketInfo einfo) {
            List<Task> s = new List<Task>();
            foreach (var x in Tasks) {
                s.Add(x.Invoke(einfo));
            }
            while (s.Any()) {
                Task finished = await Task.WhenAny(s);
                s.Remove(finished);
            }
        }
    }

}
