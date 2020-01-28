using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TShockAPI;
using TerrariaApi.Server;

namespace TerraNPCBot.Program {
    public class PluginHooks {
        public static void OnJoin(ConnectEventArgs args) {
            Program.Players[args.Who] = new BTSPlayer(args.Who);
            if (Program.Bots[args.Who] != null) {
                Program.Bots[args.Who]._actuallyJoined = true;
            }
        }

        public static void OnLeave(LeaveEventArgs args) {
            if (Program.Bots[args.Who] == null && Program.Players[args.Who]._autosave) {
                Utils.StreamWriter.BTSPlayerToStream(Program.Players[args.Who]);
            }
            else if (Program.Bots[args.Who] != null) {
                Program.Bots[args.Who].Stop();
            }
            Program.Players[args.Who] = null;
        } 

        public static void OnGetData(GetDataEventArgs args) {
            Bot p = Program.Players[args.Msg.whoAmI]?._ownedBots?.FirstOrDefault(x => x._recording);
            if (p != null) {
                using (MemoryStream m = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)) {

                    if (args.Msg.whoAmI != p._owner) return;

                    if (p._timerBetweenPackets == null) {
                        p._timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p._timerBetweenPackets.Start();

                        p._previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Array.Copy(args.Msg.readBuffer, args.Index, p._previousPacket.Buffer, 0, args.Length);
                    }  // first packet recieved
                    else {
                        p._timerBetweenPackets.Stop();
                        p._recordedPackets.Add(new RecordedPacket(p._previousPacket, (uint)p._timerBetweenPackets.ElapsedMilliseconds));  // 592 hours at int limit, casting shan't be a problem

                        p._timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p._timerBetweenPackets.Start();

                        p._previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Array.Copy(args.Msg.readBuffer, args.Index, p._previousPacket.Buffer, 0, args.Length);
                    }  // all packets onwards
                }
            }

        }
    }
}
