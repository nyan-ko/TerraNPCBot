using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TShockAPI;
using TerrariaApi.Server;

namespace rt.Program {
    public class PluginHooks {
        public static void OnJoin(JoinEventArgs args) {
            Program.Players[args.Who] = new BTSPlayer(args.Who);
        }

        public static void OnLeave(LeaveEventArgs args) {
            Program.Players[args.Who] = new BTSPlayer(args.Who);
        }

        public static void OnGetData(GetDataEventArgs args) {
            Bot p = Program.Players[args.Msg.whoAmI]?._ownedBots?.FirstOrDefault(x => x._recording);
            if (p != null) {
                using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))) {

                    if (args.Msg.whoAmI != p._owner) return;

                    var packet = PacketBase.Parse(reader, p._player, p._world, args);
                    if (packet != null) {
                        if (!p._timerBetweenPackets.IsRunning) {
                            p._timerBetweenPackets = new System.Diagnostics.Stopwatch();
                            p._timerBetweenPackets.Start();

                            p._previousPacket = packet;
                        }  // first packet recieved
                        else {
                            p._timerBetweenPackets.Stop();
                            p._recordedPackets.Add(new RecordedPacket(p._previousPacket, (int)p._timerBetweenPackets.ElapsedMilliseconds));  // 592 hours at int limit, casting shan't be a problem

                            p._timerBetweenPackets = new System.Diagnostics.Stopwatch();
                            p._timerBetweenPackets.Start();

                            p._previousPacket = packet;
                        }  // all packets onwards
                    }
                }
            }

        }
    }
}
