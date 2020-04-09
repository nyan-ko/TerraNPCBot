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
        public static void OnJoin(GreetPlayerEventArgs args) {
            Program.Players[args.Who] = new BTSPlayer(args.Who);          
        }

        public static void OnLeave(LeaveEventArgs args) {
            if (Program.Players[args.Who] == null)
                return;
            if (Program.Bots[args.Who] == null && Program.Players[args.Who].autosave) {
                Utils.FileWriter.BTSPlayerToStream(Program.Players[args.Who]);
            }            
            Program.Players[args.Who] = null;
        } 

        public static void OnGetData(GetDataEventArgs args) {
            Bot p = Program.Players[args.Msg.whoAmI]?.ownedBots?.FirstOrDefault(x => x.recording);
            if (p != null) {
                using (MemoryStream m = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)) {

                    if (args.Msg.whoAmI != p.owner) return;

                    if (p.timerBetweenPackets == null) {
                        p.timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p.timerBetweenPackets.Start();

                        p.previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Buffer.BlockCopy(args.Msg.readBuffer, args.Index, p.previousPacket.Buffer, 0, args.Length);
                    }  // first packet recieved
                    else {
                        p.timerBetweenPackets.Stop();
                        p.recordedPackets.Add(new RecordedPacket(p.previousPacket, (uint)p.timerBetweenPackets.ElapsedMilliseconds));

                        p.timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p.timerBetweenPackets.Start();

                        p.previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Buffer.BlockCopy(args.Msg.readBuffer, args.Index, p.previousPacket.Buffer, 0, args.Length);
                    }  // all packets onwards
                }
            }

        }
    }
}
